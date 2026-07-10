import { expect, test, type APIRequestContext, type APIResponse } from '@playwright/test';

const missingId = 2147483647;

type CrudEndpoint = {
  name: string;
  collectionPath: string;
  byIdPath: (id: number) => string;
  createPayload: Record<string, unknown>;
  updatePayload: Record<string, unknown>;
};

type RequestOptions = {
  data?: Record<string, unknown>;
  params?: Record<string, string>;
  maxRedirects?: number;
};

const crudEndpoints: CrudEndpoint[] = [
  {
    name: 'devices',
    collectionPath: '/devices/api',
    byIdPath: (id) => `/devices/api/${id}`,
    createPayload: {
      Name: 'PW Device',
      Manufacturer: 'Playwright',
      SerialNumber: 'PW-DEVICE-001',
      PurchaseDate: '2026-06-11T00:00:00Z',
      MeasurementType: 0,
    },
    updatePayload: {
      Name: 'PW Device Updated',
      Manufacturer: 'Playwright Updated',
      SerialNumber: 'PW-DEVICE-002',
      PurchaseDate: '2026-06-12T00:00:00Z',
      MeasurementType: 1,
    },
  },
  {
    name: 'laboratories',
    collectionPath: '/laboratories/api',
    byIdPath: (id) => `/laboratories/api/${id}`,
    createPayload: {
      Name: 'PW Lab',
      Location: 'Building PW',
      BuildingCode: 'PW1',
      RoomNumber: 101,
      ResponsiblePerson: 'Playwright Lab Owner',
    },
    updatePayload: {
      Name: 'PW Lab Updated',
      Location: 'Building PW2',
      BuildingCode: 'PW2',
      RoomNumber: 202,
      ResponsiblePerson: 'Playwright Updated Owner',
    },
  },
  {
    name: 'technicians',
    collectionPath: '/technicians/api',
    byIdPath: (id) => `/technicians/api/${id}`,
    createPayload: {
      Name: 'PW Technician',
      Email: 'playwright.tech@example.com',
      PhoneNumber: '555-1000',
      Certification: 'PW Certification',
      YearsOfExperience: 4,
    },
    updatePayload: {
      Name: 'PW Technician Updated',
      Email: 'playwright.tech.updated@example.com',
      PhoneNumber: '555-1001',
      Certification: 'PW Certification Updated',
      YearsOfExperience: 5,
    },
  },
  {
    name: 'calibrations',
    collectionPath: '/calibrations/api',
    byIdPath: (id) => `/calibrations/api/${id}`,
    createPayload: {
      DeviceId: 1,
      TechnicianId: 1,
      CalibrationDateTime: '2026-06-11T10:30:00Z',
      CalibrationStandard: 'ISO 17025',
      MeasuredDeviation: 0.05,
      PassedCalibration: true,
      NextCalibrationDue: '2027-06-11T00:00:00Z',
      Notes: 'Playwright create calibration payload',
    },
    updatePayload: {
      DeviceId: 1,
      TechnicianId: 1,
      CalibrationDateTime: '2026-06-12T10:30:00Z',
      CalibrationStandard: 'ISO 17025 Updated',
      MeasuredDeviation: 0.02,
      PassedCalibration: false,
      NextCalibrationDue: '2027-12-11T00:00:00Z',
      Notes: 'Playwright update calibration payload',
    },
  },
  {
    name: 'device locations',
    collectionPath: '/devicelocations/api',
    byIdPath: (id) => `/devicelocations/api/${id}`,
    createPayload: {
      DeviceId: 1,
      LaboratoryId: 1,
      AssignedDate: '2026-06-11T08:00:00Z',
      RemovedDate: null,
      IsCurrentLocation: true,
      AssignmentReason: 'Playwright create assignment',
    },
    updatePayload: {
      DeviceId: 1,
      LaboratoryId: 1,
      AssignedDate: '2026-06-12T09:00:00Z',
      RemovedDate: '2026-06-13T09:00:00Z',
      IsCurrentLocation: false,
      AssignmentReason: 'Playwright update assignment',
    },
  },
];

const lookupEndpoints = ['/lookup/devices', '/lookup/technicians', '/lookup/laboratories'];

function expectAuthGate(response: APIResponse, endpoint: string): void {
  const status = response.status();
  expect([401, 403, 302]).toContain(status);

  if (status === 302) {
    const location = response.headers()['location'] ?? '';
    expect(location.toLowerCase(), `Expected login redirect for ${endpoint}`).toContain('/account/login');
  }
}

async function sendRequest(
  request: APIRequestContext,
  method: 'GET' | 'POST' | 'PUT' | 'DELETE',
  endpoint: string,
  options?: RequestOptions,
): Promise<APIResponse> {
  let response: APIResponse;
  if (method === 'GET') {
    response = await request.get(endpoint, options);
  } else if (method === 'POST') {
    response = await request.post(endpoint, options);
  } else if (method === 'PUT') {
    response = await request.put(endpoint, options);
  } else {
    response = await request.delete(endpoint, options);
  }

  return response;
}

async function getFirstEntityIdOrNull(request: APIRequestContext, collectionPath: string): Promise<number | null> {
  const response = await sendRequest(request, 'GET', collectionPath);
  expect(response.status(), `GET ${collectionPath} should return 200`).toBe(200);

  const body = (await response.json()) as Array<{ id?: unknown }>;
  expect(Array.isArray(body), `GET ${collectionPath} should return an array`).toBeTruthy();

  if (body.length === 0 || typeof body[0]?.id !== 'number') {
    return null;
  }

  return body[0].id;
}

test.describe('API endpoints coverage', () => {
  test('covers all GET API and lookup endpoints', async ({ request }) => {
    for (const endpoint of crudEndpoints) {
      const listResponse = await sendRequest(request, 'GET', endpoint.collectionPath);
      expect(listResponse.status(), `GET ${endpoint.collectionPath}`).toBe(200);

      const listBody = await listResponse.json();
      expect(Array.isArray(listBody), `GET ${endpoint.collectionPath} should return array`).toBeTruthy();

      const firstId = await getFirstEntityIdOrNull(request, endpoint.collectionPath);
      const idToQuery = firstId ?? missingId;
      const byIdResponse = await sendRequest(request, 'GET', endpoint.byIdPath(idToQuery));

      if (firstId === null) {
        expect(byIdResponse.status(), `GET ${endpoint.byIdPath(idToQuery)} when collection empty`).toBe(404);
      } else {
        expect(byIdResponse.status(), `GET ${endpoint.byIdPath(idToQuery)}`).toBe(200);
        const byIdBody = (await byIdResponse.json()) as { id?: unknown };
        expect(byIdBody.id).toBe(firstId);
      }
    }

    for (const lookupPath of lookupEndpoints) {
      const lookupResponse = await sendRequest(request, 'GET', lookupPath, { params: { term: 'a' } });
      expect(lookupResponse.status(), `GET ${lookupPath}`).toBe(200);
      const lookupBody = await lookupResponse.json();
      expect(Array.isArray(lookupBody), `GET ${lookupPath} should return array`).toBeTruthy();
    }
  });

  test('covers all protected POST/PUT/DELETE API endpoints', async ({ request }) => {
    for (const endpoint of crudEndpoints) {
      const postResponse = await sendRequest(request, 'POST', endpoint.collectionPath, {
        data: endpoint.createPayload,
        maxRedirects: 0,
      });
      expectAuthGate(postResponse, `POST ${endpoint.collectionPath}`);

      const putResponse = await sendRequest(request, 'PUT', endpoint.byIdPath(missingId), {
        data: {
          ...endpoint.updatePayload,
          Id: missingId,
        },
        maxRedirects: 0,
      });
      expectAuthGate(putResponse, `PUT ${endpoint.byIdPath(missingId)}`);

      const deleteResponse = await sendRequest(request, 'DELETE', endpoint.byIdPath(missingId), {
        maxRedirects: 0,
      });
      expectAuthGate(deleteResponse, `DELETE ${endpoint.byIdPath(missingId)}`);
    }
  });
});

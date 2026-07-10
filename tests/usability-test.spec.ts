import { test, expect } from '@playwright/test';

test('home page loads key dashboard tables', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Metrology Instrumentation Dashboard' })).toBeVisible();
  await expect(page.getByRole('columnheader', { name: 'Technician Name' })).toBeVisible();
  await expect(page.getByRole('columnheader', { name: 'Certification' })).toBeVisible();
  await expect(page.getByRole('columnheader', { name: 'Date & Time' })).toBeVisible();
});

test('user can sign in and sign out from navigation', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('link', { name: 'Sign in' }).click();
  await expect(page.getByRole('heading', { name: 'Sign in' })).toBeVisible();

  await page.getByRole('textbox', { name: 'Email' }).fill('admin@admin.com');
  await page.getByRole('textbox', { name: 'Password' }).fill('admin06');
  await page.getByRole('button', { name: 'Sign in' }).click();

  await expect(page.getByRole('button', { name: 'Sign out' })).toBeVisible();
  await page.getByRole('button', { name: 'Sign out' }).click();
  await expect(page.getByRole('link', { name: 'Sign in' })).toBeVisible();
});

test('technician list supports search and clear interactions', async ({ page }) => {
  await page.goto('/technicians');

  await expect(page.getByRole('heading', { name: 'Technicians' })).toBeVisible();
  await expect(page.getByRole('columnheader', { name: 'Name' })).toBeVisible();

  const searchInput = page.locator('#technician-search');
  await searchInput.fill('John');
  await expect(page.locator('#technician-table-container')).toContainText('John');

  await page.getByRole('button', { name: 'Clear' }).click();
  await expect(searchInput).toHaveValue('');
  await expect(page.locator('#technician-table-container')).toContainText('John');
});
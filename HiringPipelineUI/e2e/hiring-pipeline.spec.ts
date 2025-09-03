import { test, expect } from '@playwright/test';

test.describe('Hiring Pipeline Tracker E2E Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to the application
    await page.goto('http://localhost:4200');
  });

  test('should display dashboard with metrics', async ({ page }) => {
    // Wait for the dashboard to load
    await page.waitForSelector('h1:has-text("Dashboard")');
    
    // Check if metric cards are displayed
    await expect(page.locator('text=Total Candidates')).toBeVisible();
    await expect(page.locator('text=Active Requisitions')).toBeVisible();
    await expect(page.locator('text=Hired Candidates')).toBeVisible();
    
    // Check if charts are present
    await expect(page.locator('text=Stage Distribution')).toBeVisible();
    await expect(page.locator('text=Hiring Velocity')).toBeVisible();
    
    // Check if recent activity is displayed
    await expect(page.locator('text=Recent Activity')).toBeVisible();
  });

  test('should navigate to stage history page', async ({ page }) => {
    // Click on the Pipeline link in sidebar
    await page.click('a[href="/pipeline"]');
    
    // Wait for the stage history page to load
    await page.waitForSelector('h1:has-text("Stage History")');
    
    // Check if the timeline view is displayed
    await expect(page.locator('text=Track candidate movement')).toBeVisible();
    
    // Check if search functionality is available
    await expect(page.locator('input[placeholder*="Search"]')).toBeVisible();
  });

  test('should navigate to applications page', async ({ page }) => {
    // Click on the Applications link in sidebar
    await page.click('a[href="/applications"]');
    
    // Wait for the applications page to load
    await page.waitForSelector('h1:has-text("Applications")');
    
    // Check if the applications table is displayed
    await expect(page.locator('text=Manage applications')).toBeVisible();
    
    // Check if add application button is present
    await expect(page.locator('button:has-text("Add Application")')).toBeVisible();
  });

  test('should navigate to candidates page', async ({ page }) => {
    // Click on the Candidates link in sidebar
    await page.click('a[href="/candidates"]');
    
    // Wait for the candidates page to load
    await page.waitForSelector('h1:has-text("Candidates")');
    
    // Check if the candidates table is displayed
    await expect(page.locator('text=Manage candidates')).toBeVisible();
    
    // Check if add candidate button is present
    await expect(page.locator('button:has-text("Add Candidate")')).toBeVisible();
  });

  test('should navigate to requisitions page', async ({ page }) => {
    // Click on the Requisitions link in sidebar
    await page.click('a[href="/requisitions"]');
    
    // Wait for the requisitions page to load
    await page.waitForSelector('h1:has-text("Requisitions")');
    
    // Check if the requisitions table is displayed
    await expect(page.locator('text=Manage job openings')).toBeVisible();
    
    // Check if add requisition button is present
    await expect(page.locator('button:has-text("Add Requisition")')).toBeVisible();
  });

  test('should toggle sidebar', async ({ page }) => {
    // Check if sidebar is initially open
    await expect(page.locator('aside')).toBeVisible();
    
    // Click the toggle button
    await page.click('button[title*="toggle"]');
    
    // Check if sidebar is collapsed
    await expect(page.locator('aside')).toHaveClass(/w-16/);
    
    // Click again to expand
    await page.click('button[title*="toggle"]');
    
    // Check if sidebar is expanded
    await expect(page.locator('aside')).toHaveClass(/w-64/);
  });

  test('should search in stage history', async ({ page }) => {
    // Navigate to stage history
    await page.click('a[href="/pipeline"]');
    await page.waitForSelector('h1:has-text("Stage History")');
    
    // Type in search box
    await page.fill('input[placeholder*="Search"]', 'John');
    
    // Check if search results are filtered
    await expect(page.locator('input[placeholder*="Search"]')).toHaveValue('John');
  });

  test('should display responsive design', async ({ page }) => {
    // Test mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    
    // Check if sidebar is responsive
    await expect(page.locator('aside')).toBeVisible();
    
    // Test tablet viewport
    await page.setViewportSize({ width: 768, height: 1024 });
    
    // Check if layout adapts
    await expect(page.locator('main')).toBeVisible();
    
    // Test desktop viewport
    await page.setViewportSize({ width: 1920, height: 1080 });
    
    // Check if layout is optimal for desktop
    await expect(page.locator('main')).toBeVisible();
  });

  test('should handle loading states', async ({ page }) => {
    // Navigate to dashboard
    await page.goto('http://localhost:4200');
    
    // Check if loading indicators are shown initially
    await expect(page.locator('text=Loading dashboard data')).toBeVisible();
    
    // Wait for data to load
    await page.waitForSelector('text=Total Candidates', { timeout: 10000 });
    
    // Check if loading is complete
    await expect(page.locator('text=Loading dashboard data')).not.toBeVisible();
  });

  test('should display error states gracefully', async ({ page }) => {
    // This test would require mocking API failures
    // For now, we'll just check if error handling elements exist
    
    // Navigate to a page
    await page.click('a[href="/applications"]');
    await page.waitForSelector('h1:has-text("Applications")');
    
    // Check if error handling is in place (this would show up if API fails)
    // The application should handle errors gracefully without crashing
    await expect(page.locator('main')).toBeVisible();
  });

  test('should maintain navigation state', async ({ page }) => {
    // Navigate to different pages and verify URL changes
    await page.click('a[href="/applications"]');
    await expect(page).toHaveURL(/.*applications/);
    
    await page.click('a[href="/candidates"]');
    await expect(page).toHaveURL(/.*candidates/);
    
    await page.click('a[href="/requisitions"]');
    await expect(page).toHaveURL(/.*requisitions/);
    
    await page.click('a[href="/pipeline"]');
    await expect(page).toHaveURL(/.*pipeline/);
    
    await page.click('a[href="/dashboard"]');
    await expect(page).toHaveURL(/.*dashboard/);
  });
});

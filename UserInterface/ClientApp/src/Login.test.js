import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import LoginPage from './LoginPage';

// Mock the navigate function
const mockNavigate = jest.fn();

// Replace the actual navigate function with the mock function
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

// Mock user data for testing purposes
jest.mock('./Users.json', () => ({
  users: [
    {
      email: 'test@example.com',
      password: 'test123',
    },
  ],
}));

test('renders Login page with form elements', () => {
  render(
    <BrowserRouter>
      <LoginPage />
    </BrowserRouter>
  );

  // Check if the form elements are rendered
  const emailInput = screen.getByPlaceholderText(/Email/i);
  const passwordInput = screen.getByPlaceholderText(/Password/i);

  // Assert that the form elements are present
  expect(emailInput).toBeInTheDocument();
  expect(passwordInput).toBeInTheDocument();
});

test('updates form inputs on user input', () => {
  render(
    <BrowserRouter>
      <LoginPage />
    </BrowserRouter>
  );

  // Check if form inputs update on user input
  const emailInput = screen.getByPlaceholderText(/Email/i);
  const passwordInput = screen.getByPlaceholderText(/Password/i);

  fireEvent.change(emailInput, { target: { value: 'test@example.com' } });
  fireEvent.change(passwordInput, { target: { value: 'test123' } });

  expect(emailInput.value).toBe('test@example.com');
  expect(passwordInput.value).toBe('test123');
});

test('handles login correctly', async () => {
  render(
    <BrowserRouter>
      <LoginPage />
    </BrowserRouter>
  );

  // Check if login is handled correctly
  const emailInput = screen.getByPlaceholderText(/Email/i);
  const passwordInput = screen.getByPlaceholderText(/Password/i);
  const loginButton = screen.getByRole('button', { name: /Login/i }); // Use getByRole

  fireEvent.change(emailInput, { target: { value: 'test@example.com' } });
  fireEvent.change(passwordInput, { target: { value: 'test123' } });

  fireEvent.click(loginButton);

  // Wait for asynchronous code to complete
  await waitFor(() => {
    // Assert that the navigate function is called with the correct argument
    expect(mockNavigate).toHaveBeenCalledWith('/app');
  });
});

test('handles empty fields correctly', async () => {
  render(
    <BrowserRouter>
      <LoginPage />
    </BrowserRouter>
  );

  // Check if login is handled correctly with empty fields
  const loginButton = screen.getByRole('button', { name: /Login/i }); // Use getByRole

  fireEvent.click(loginButton);

  // Wait for asynchronous code to complete
  await waitFor(() => {
    // Assert that the navigate function is not called
    expect(mockNavigate).not.toHaveBeenCalled();
  });
});

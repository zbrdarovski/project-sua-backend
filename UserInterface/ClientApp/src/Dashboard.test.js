import React from 'react';
import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import Dashboard from './Dashboard';

test('renders Dashboard component with buttons', () => {
  render(
    <BrowserRouter>
      <Dashboard />
    </BrowserRouter>
  );

  // Check if the buttons are rendered
  const profileButton = screen.getByText(/Profil/i);
  const usersButton = screen.getByText(/Uporabniki/i);
  const equipmentButton = screen.getByText(/Inventar/i);
  const registerButton = screen.getByText(/Register/i);

  expect(profileButton).toBeInTheDocument();
  expect(usersButton).toBeInTheDocument();
  expect(equipmentButton).toBeInTheDocument();
  expect(registerButton).toBeInTheDocument();
});

test('links to the correct routes', () => {
  render(
    <BrowserRouter>
      <Dashboard />
    </BrowserRouter>
  );

  // Check if the buttons link to the correct routes
  const profileButton = screen.getByText(/Profil/i).closest('a');
  const usersButton = screen.getByText(/Uporabniki/i).closest('a');
  const equipmentButton = screen.getByText(/Inventar/i).closest('a');
  const registerButton = screen.getByText(/Register/i).closest('a');

  expect(profileButton).toHaveAttribute('href', '/profile');
  expect(usersButton).toHaveAttribute('href', '/users');
  expect(equipmentButton).toHaveAttribute('href', '/equipment');
  expect(registerButton).toHaveAttribute('href', '/register');
});

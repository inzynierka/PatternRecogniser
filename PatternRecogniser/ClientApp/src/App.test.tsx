import React from 'react';
import App from './App';
import {render, screen} from "@testing-library/react";
import '@testing-library/jest-dom'

test('renders learn react link', () => {
    render(<App/>);
    const linkElement = screen.getByText(/Good luck/i);
    expect(linkElement).toBeInTheDocument();
});
import '@testing-library/jest-dom';

import { render, screen } from '@testing-library/react';
import React from 'react';

import MyAccountPage from '../pages/Account/MyAccount';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};
  
describe("MyAccountPanel", () => {
    it("renders my account panel", () => {
        render(<MyAccountPage />);
        expect(screen.getByText("Moje konto")).toBeInTheDocument();
    });

    it("displays login and email", () => {
        render(<MyAccountPage />);
        expect(screen.getByText("Login:")).toBeInTheDocument();
        expect(screen.getByText("Adres e-mail:")).toBeInTheDocument();
    });

    it("displays change password button", () => {
        render(<MyAccountPage />);
        expect(screen.getByText("Zmień hasło")).toBeInTheDocument();
    });
})

import '@testing-library/jest-dom';

import { fireEvent, screen, waitFor } from '@testing-library/react';
import { Menu } from 'react-bootstrap/lib/Dropdown';

import Login from '../pages/Login';
import { allowsTypingIn, mockedUseNavigate, reactsOnClicking, renderComponentWithRouter, requiresNotEmpty } from './util';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};
  
describe("LoginPanel", () => {
    it("renders login panel", () => {
        renderComponentWithRouter(<Login />);
        expect(screen.getByText("Logowanie")).toBeInTheDocument();
    });
    it("should display blank login form", async () => {
        const { findByTestId } = renderComponentWithRouter(<Login/>);
        const loginForm = await findByTestId("login-form");
      
        expect(loginForm).toHaveFormValues({
            "login-input": "",
            "password-input": ""
        });
    });
   
    test("no menu items are displayed", async () => {
        const { findByTestId } = renderComponentWithRouter(<Login/>);
        const menuItems = await findByTestId("main-menu").catch(() => { return null; });
        expect(menuItems).toBe(null);
    });

    it("allows typing in login", async () => {
        allowsTypingIn("login-input", <Login />);
    });
    it("allows typing in password", async () => {
        allowsTypingIn("password-input", <Login />);
    });

    it("requires login", async () => {
        requiresNotEmpty("login-input", "login-form", <Login/>)
    });
    it("requires password", async () => {
        requiresNotEmpty("password-input", "login-form", <Login/>)
    });

    it("invalid user can't log in", async () => {
        const { findByTestId } = renderComponentWithRouter(<Login/>);
        const loginForm = await findByTestId("login-form");
        const loginInput = await findByTestId("login-input");
        const passwordInput = await findByTestId("password-input");
      
        fireEvent.change(loginInput, { target: { value: "invalidLogin" } });
        fireEvent.change(passwordInput, { target: { value: "invalidPassword" } });

        expect(loginForm).toHaveFormValues({
            "login-input": "invalidLogin",
            "password-input": "invalidPassword"
        });
      
        fireEvent.submit(loginForm);
        
        await waitFor(() => {
            expect(mockedUseNavigate).toHaveBeenCalledTimes(0);
            expect(screen.getByText("Niepoprawne dane")).toBeInTheDocument();
        });
    });

    it("signin button reacts on clicking", async () => {
        reactsOnClicking("signin-button", <Login/>);
    });
    it("login button reacts to clicking", async () => {
        reactsOnClicking("login-button", <Login/>);
    });
})
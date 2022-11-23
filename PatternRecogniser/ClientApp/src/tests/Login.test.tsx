import '@testing-library/jest-dom';

import { fireEvent, screen, waitFor } from '@testing-library/react';
import { act } from 'react-dom/test-utils';

import Login from '../pages/Login';
import { allowsTypingIn, mockedUseNavigate, reactsOnClicking, renderComponentWithRouter, requiresNotEmpty } from './util';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};

const mockedUsedNavigate = jest.fn();

jest.mock('react-router-dom', () => ({
   ...jest.requireActual('react-router-dom') as any,
  useNavigate: () => mockedUsedNavigate,
}));

describe("LoginPanel", () => {
    it("renders login panel", () => {
        renderComponentWithRouter(<Login />);
        expect(screen.getByText("Logowanie")).toBeInTheDocument();
    });

    it("displays login and password inputs", async () => {
        const { findByTestId } = renderComponentWithRouter(<Login />);
        const loginInput = await findByTestId("login-input");
        const passwordInput = await findByTestId("password-input");
        expect(loginInput).toBeInTheDocument();
        expect(passwordInput).toBeInTheDocument();
    });
    it("displays login and signin buttons", async () => {
        const { findByTestId } = renderComponentWithRouter(<Login />);
        const loginButton = await findByTestId("login-button");
        const signinButton = await findByTestId("signin-button");
        expect(loginButton).toBeInTheDocument();
        expect(signinButton).toBeInTheDocument();
    });
    it("displays blank login form", async () => {
        const { findByTestId } = renderComponentWithRouter(<Login/>);
        const loginForm = await findByTestId("login-form");
      
        expect(loginForm).toHaveFormValues({
            "login-input": "",
            "password-input": ""
        });
    });
    test("does not display menu items", async () => {
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

    it("requires login to be not empty", async () => {
        let otherElements = { "password-input": "testinput" };
        requiresNotEmpty("login-input", otherElements, "login-form", <Login/>)
    });
    it("requires password to be not empty", async () => {
        let otherElements = { "login-input": "testinput" };
        requiresNotEmpty("password-input", otherElements, "login-form", <Login/>)
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

    it("signin button reacts to click", async () => {
        reactsOnClicking("signin-button", <Login/>);
    });
    it("login button reacts to click", async () => {
        reactsOnClicking("login-button", <Login/>);
    });

    it("valid user can log in", async () => {
        const { findByTestId } = renderComponentWithRouter(<Login/>);
        const loginForm = await findByTestId("login-form");
        const loginInput = await findByTestId("login-input");
        const passwordInput = await findByTestId("password-input");
      
        fireEvent.change(loginInput, { target: { value: "admin" } });
        fireEvent.change(passwordInput, { target: { value: "admin" } });

        expect(loginForm).toHaveFormValues({
            "login-input": "admin",
            "password-input": "admin"
        });

        fireEvent.submit(loginForm);

        await waitFor(() => {
            // data is saved in local storage ONLY after successful login
            expect(localStorage.getItem("userId")).toBe("admin");
        });
    });
})
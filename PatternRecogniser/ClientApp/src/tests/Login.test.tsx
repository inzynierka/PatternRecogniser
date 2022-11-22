import '@testing-library/jest-dom';

import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';

import Login from '../pages/Login';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};


// mock useNavigate
const mockedUseNavigate = jest.fn()
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: () => mockedUseNavigate,
  })
);

function renderLogin() {
    return render(
        <BrowserRouter>
            <Login />
        </BrowserRouter>,
    );
  }
  
describe("LoginPanel", () => {
    it("renders login panel", () => {
        renderLogin();
        expect(screen.getByText("Logowanie")).toBeInTheDocument();
    });
    it("should display blank login form", async () => {
        const { findByTestId } = renderLogin();
        const loginForm = await findByTestId("login-form");
      
        expect(loginForm).toHaveFormValues({
            "login-input": "",
            "password-input": ""
        });
    })

    it("allows typing in login", async () => {
        const { findByTestId } = renderLogin();
        const loginForm = await findByTestId("login-form");
        const loginInput = await findByTestId("login-input");
      
        expect(loginForm).toHaveFormValues({
            "login-input": "",
            "password-input": ""
        });
      
        fireEvent.change(loginInput, { target: { value: "abc" } });
      
        expect(loginForm).toHaveFormValues({
            "login-input": "abc",
            "password-input": ""
        });
    })
    it("allows typing in password", async () => {
        const { findByTestId } = renderLogin();
        const loginForm = await findByTestId("login-form");
        const passwordInput = await findByTestId("password-input");
      
        expect(loginForm).toHaveFormValues({
            "login-input": "",
            "password-input": ""
        });
      
        fireEvent.change(passwordInput, { target: { value: "abc" } });
      
        expect(loginForm).toHaveFormValues({
            "login-input": "",
            "password-input": "abc"
        });
    })

    it("requires login", async () => {
        const { findByTestId } = renderLogin();
        const loginForm = await findByTestId("login-form");
        const passwordInput = await findByTestId("password-input");

        fireEvent.change(passwordInput, { target: { value: "abc" } });

        expect(loginForm).toHaveFormValues({
            "login-input": "",
            "password-input": "abc"
        });

        fireEvent.submit(loginForm);

        await waitFor(() => { expect(mockedUseNavigate).toHaveBeenCalledTimes(0); });
    })
    it("requires password", async () => {
        const { findByTestId } = renderLogin();
        const loginForm = await findByTestId("login-form");
        const loginInput = await findByTestId("login-input");

        fireEvent.change(loginInput, { target: { value: "abc" } });
      
        expect(loginForm).toHaveFormValues({
            "login-input": "abc",
            "password-input": ""
        });

        fireEvent.submit(loginForm);

        await waitFor(() => { expect(mockedUseNavigate).toHaveBeenCalledTimes(0); });
    })

    it("invalid user can't log in", async () => {
        const { findByTestId } = renderLogin();
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
    })

    it("signin button reacts to clicking", async () => {
        const { findByTestId } = renderLogin();
        const signinButton = await findByTestId("signin-button");
        let signInHandlerCalled = 0;

        signinButton.addEventListener('click', () => { signInHandlerCalled++; });

        fireEvent.click(signinButton);

        expect(signInHandlerCalled).toBe(1);
    })
    it("login button reacts to clicking", async () => {
        const { findByTestId } = renderLogin();
        const loginButton = await findByTestId("login-button");
        let loginHandlerCalled = 0;

        loginButton.addEventListener('click', () => { loginHandlerCalled++; });

        fireEvent.click(loginButton);

        expect(loginHandlerCalled).toBe(1);
    })

    it("no menu items are displayed", async () => {
        const { findByTestId } = renderLogin();
        const menuItems = await findByTestId("main-menu").catch(() => { return null; });
        expect(menuItems).toBe(null);
    })
})

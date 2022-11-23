import '@testing-library/jest-dom';

import { fireEvent, screen, waitFor } from '@testing-library/react';

import Signin from '../pages/Signin';
import { allowsTypingIn, mockedUseNavigate, renderComponentWithRouter, requiresNotEmpty } from './util';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};


jest.mock('react-password-checklist', () => ({
    ...jest.mock('react-password-checklist'),
    PasswordChecklist : () => <div />
  })
);

  
describe("SigninPanel", () => {
    it("renders signin panel", () => {
        renderComponentWithRouter(<Signin />);
        expect(screen.getByText("Rejestracja")).toBeInTheDocument();
    });

    test("does not display menu items", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin/>);
        const menuItems = await findByTestId("main-menu").catch(() => { return null; });
        expect(menuItems).toBe(null);
    });
    it("displays all form inputs", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const emailInput = await findByTestId("email-input");
        const loginInput = await findByTestId("login-input");
        const passwordInput = await findByTestId("password-input");
        const passwordConfirmInput = await findByTestId("password-confirm-input");
        expect(emailInput).toBeInTheDocument();
        expect(loginInput).toBeInTheDocument();
        expect(passwordInput).toBeInTheDocument();
        expect(passwordConfirmInput).toBeInTheDocument();
    });
    it("displays signin and cancel buttons", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const signinButton = await findByTestId("signin-button");
        const cancelButton = await findByTestId("cancel-button");
        expect(signinButton).toBeInTheDocument();
        expect(cancelButton).toBeInTheDocument();
    });
    it("displays blank signin form", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin/>);
        const signinForm = await findByTestId("signin-form");

        expect(signinForm).toHaveFormValues({
            "email-input": "",
            "login-input": "",
            "password-input": "",
            "password-confirm-input": ""
        });
    });

    it("allows typing in email", async () => {
        allowsTypingIn("email-input", <Signin />);
    });
    it("allows typing in login", async () => {
        allowsTypingIn("login-input", <Signin />);
    });
    it("allows typing in password", async () => {
        allowsTypingIn("password-input", <Signin />);
    });
    it("allows typing in password confirmation", async () => {
        allowsTypingIn("password-confirm-input", <Signin />);
    });

    it("requires email to be not empty", async () => {
        const otherElements = {
            "login-input": "testinput",
            "password-input": "testinput",
            "password-confirm-input": "testinput"
        };
        requiresNotEmpty("email-input", otherElements, "signin-form", <Signin />);
    });
    it("requires login to be not empty", async () => {
        const otherElements = {
            "email-input": "testinput",
            "password-input": "testinput",
            "password-confirm-input": "testinput"
        };
        requiresNotEmpty("login-input", otherElements, "signin-form", <Signin />);
    });
    it("requires password to be not empty", async () => {
        const otherElements = {
            "email-input": "testinput",
            "login-input": "testinput",
            "password-confirm-input": "testinput"
        };
        requiresNotEmpty("password-input", otherElements, "signin-form", <Signin />);
    });
    it("requires password confirmation to be not empty", async () => {
        const otherElements = {
            "email-input": "testinput",
            "login-input": "testinput",
            "password-input": "testinput"
        };
        requiresNotEmpty("password-confirm-input", otherElements, "signin-form", <Signin />);
    });

    it("requires password to be at least 8 characters long", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const signinForm = await findByTestId("signin-form");
        const loginInput = await findByTestId("login-input");
        const emailInput = await findByTestId("email-input");
        const passwordInput = await findByTestId("password-input");
        const passwordConfirmInput = await findByTestId("password-confirm-input");
        const signinButton = await findByTestId("signin-button");

        fireEvent.change(loginInput, { target: { value: "login" } });
        fireEvent.change(emailInput, { target: { value: "email@email.com" } });
        fireEvent.change(passwordInput, { target: { value: "1aB#56" } });
        fireEvent.change(passwordConfirmInput, { target: { value: "1aB#56" } });

        signinButton.click();

        expect(mockedUseNavigate).toBeCalledTimes(0);
    });
    it("requires password to contain at least one uppercase letter", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const signinForm = await findByTestId("signin-form");
        const loginInput = await findByTestId("login-input");
        const emailInput = await findByTestId("email-input");
        const passwordInput = await findByTestId("password-input");
        const passwordConfirmInput = await findByTestId("password-confirm-input");
        const signinButton = await findByTestId("signin-button");

        fireEvent.change(loginInput, { target: { value: "login" } });
        fireEvent.change(emailInput, { target: { value: "email@email.com" } });
        fireEvent.change(passwordInput, { target: { value: "1a#56jtshf355737$@^" } });
        fireEvent.change(passwordConfirmInput, { target: { value: "1a#56jtshf355737$@" } });

        signinButton.click();

        expect(mockedUseNavigate).toBeCalledTimes(0);
    });
    it("requires password to contain at least one lowercase letter", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const signinForm = await findByTestId("signin-form");
        const loginInput = await findByTestId("login-input");
        const emailInput = await findByTestId("email-input");
        const passwordInput = await findByTestId("password-input");
        const passwordConfirmInput = await findByTestId("password-confirm-input");
        const signinButton = await findByTestId("signin-button");

        fireEvent.change(loginInput, { target: { value: "login" } });
        fireEvent.change(emailInput, { target: { value: "email@email.com" } });
        fireEvent.change(passwordInput, { target: { value: "1A#56JTSHF355737$@^" } });
        fireEvent.change(passwordConfirmInput, { target: { value: "1A#56JTSHF355737$@" } });

        signinButton.click();

        expect(mockedUseNavigate).toBeCalledTimes(0);
    });
    it("requires password to contain at least one number", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const signinForm = await findByTestId("signin-form");
        const loginInput = await findByTestId("login-input");
        const emailInput = await findByTestId("email-input");
        const passwordInput = await findByTestId("password-input");
        const passwordConfirmInput = await findByTestId("password-confirm-input");
        const signinButton = await findByTestId("signin-button");

        fireEvent.change(loginInput, { target: { value: "login" } });
        fireEvent.change(emailInput, { target: { value: "email@email.com" } });
        fireEvent.change(passwordInput, { target: { value: "aA#JTSHFasdDAGD!$$@^" } });
        fireEvent.change(passwordConfirmInput, { target: { value: "aA#JTSHFasdDAGD!$$@^" } });

        signinButton.click();

        expect(mockedUseNavigate).toBeCalledTimes(0);
    });

    it("accepts valid data", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const signinForm = await findByTestId("signin-form");
        const loginInput = await findByTestId("login-input");
        const emailInput = await findByTestId("email-input");
        const passwordInput = await findByTestId("password-input");
        const passwordConfirmInput = await findByTestId("password-confirm-input");
        const signinButton = await findByTestId("signin-button");

        let couter = 0;
        signinForm.addEventListener("submit", (e) => {
            couter++;
        });

        fireEvent.change(loginInput, { target: { value: "login" } });
        fireEvent.change(emailInput, { target: { value: "email@email.com" } });
        fireEvent.change(passwordInput, { target: { value: "someSTRONGpassword123@!#" } });
        fireEvent.change(passwordConfirmInput, { target: { value: "someSTRONGpassword123@!#" } });

        signinButton.click();

        expect(couter).toBe(1);
    });
    it("saves data to localStorage after successful registration", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const loginInput = await findByTestId("login-input");
        const emailInput = await findByTestId("email-input");
        const passwordInput = await findByTestId("password-input");
        const passwordConfirmInput = await findByTestId("password-confirm-input");
        const signinButton = await findByTestId("signin-button");

        fireEvent.change(loginInput, { target: { value: "login" } });
        fireEvent.change(emailInput, { target: { value: "email@email.com" } });
        fireEvent.change(passwordInput, { target: { value: "someSTRONGpassword123@!#" } });
        fireEvent.change(passwordConfirmInput, { target: { value: "someSTRONGpassword123@!#" } });

        signinButton.click();

        await waitFor(() => {
            expect(localStorage.getItem("token")).not.toBeNull();
            expect(localStorage.getItem("userId")).toBe('login');
            expect(localStorage.getItem("email")).toBe("email@email.com")
        });
    });

    it("signin button reacts to click", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const signinButton = await findByTestId("signin-button");

        let couter = 0;
        signinButton.addEventListener("click", (e) => {
            couter++;
        });

        signinButton.click();

        expect(couter).toBe(1);
    });
    it("cancel button reacts to click", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const cancelButton = await findByTestId("cancel-button");

        let couter = 0;
        cancelButton.addEventListener("click", (e) => {
            couter++;
        });

        cancelButton.click();

        expect(couter).toBe(1);
    });
})

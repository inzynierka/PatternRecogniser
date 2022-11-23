import { fireEvent, render, waitFor } from '@testing-library/react';
import React from 'react';
import { MemoryRouter } from 'react-router-dom';

export function  renderComponentWithRouter(component : JSX.Element) {
    return render(
        <MemoryRouter>
            {component}
        </MemoryRouter>,
    );
}

// mock useNavigate
export const mockedUseNavigate = jest.fn()
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: () => mockedUseNavigate,
  })
);

// mock useDispatch
export const mockUseDispatch = jest.fn();
jest.mock('react-redux', () => ({
  useDispatch: () => mockUseDispatch
}));

// mock useContext
export const mockUseContext = jest.fn();
jest.mock('react', () => ({
    ...jest.requireActual('react'),
    useContext: () => mockUseContext,
}));

export const allowsTypingIn = async (inputTestId : string, component : JSX.Element) => {
    const { findByTestId } = renderComponentWithRouter(component);
    const input = await findByTestId(inputTestId);
  
    expect(input).toHaveValue("");
  
    fireEvent.change(input, { target: { value: "abc" } });
  
    expect(input).toHaveValue("abc");
}

export const requiresNotEmpty = async (inputTestId : string, otherElements : Record<string, unknown>, formTestId : string, component : JSX.Element) => {
    const { findByTestId } = renderComponentWithRouter(component);
    const loginForm = await findByTestId(formTestId);
    const input = await findByTestId(inputTestId);
  
    expect(input).toHaveValue("");

    let coutner = 0;
    loginForm.addEventListener("submit", () => {
        coutner++
    });

    fireEvent.submit(loginForm);

    await waitFor(() => { expect(coutner).toBe(0); });
}

export const reactsOnClicking = async (buttonTestId : string, component : JSX.Element) => {
    const { findByTestId } = renderComponentWithRouter(component);
    const button = await findByTestId(buttonTestId);
    let calledClickHandler = 0;

    button.addEventListener('click', () => { calledClickHandler++; });

    fireEvent.click(button);

    expect(calledClickHandler).toBe(1);
}
import { fireEvent, render, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';

export function  renderComponentWithRouter(component : JSX.Element) {
    return render(
        <BrowserRouter>
            {component}
        </BrowserRouter>,
    );
}

// mock useNavigate
export const mockedUseNavigate = jest.fn()
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: () => mockedUseNavigate,
  })
);

export const allowsTypingIn = async (inputTestId : string, component : JSX.Element) => {
    const { findByTestId } = renderComponentWithRouter(component);
    const input = await findByTestId(inputTestId);
  
    expect(input).toHaveValue("");
  
    fireEvent.change(input, { target: { value: "abc" } });
  
    expect(input).toHaveValue("abc");
}

export const requiresNotEmpty = async (inputTestId : string, formTestId : string, component : JSX.Element) => {
    const { findByTestId } = renderComponentWithRouter(component);
    const loginForm = await findByTestId(formTestId);
    const input = await findByTestId(inputTestId);
  
    expect(input).toHaveValue("");

    fireEvent.submit(loginForm);

    await waitFor(() => { expect(mockedUseNavigate).toHaveBeenCalledTimes(0); });
}

export const reactsOnClicking = async (buttonTestId : string, component : JSX.Element) => {
    const { findByTestId } = renderComponentWithRouter(component);
    const button = await findByTestId(buttonTestId);
    let calledClickHandler = 0;

    button.addEventListener('click', () => { calledClickHandler++; });

    fireEvent.click(button);

    expect(calledClickHandler).toBe(1);
}
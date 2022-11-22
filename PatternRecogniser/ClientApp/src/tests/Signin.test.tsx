import '@testing-library/jest-dom';

import { screen } from '@testing-library/react';
import { mockComponent } from 'react-dom/test-utils';

import Signin from '../pages/Signin';
import { renderComponentWithRouter } from './util';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};

//export const mockedUseNavigate = jest.fn()
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

    it("no menu items are displayed", async () => {
        const { findByTestId } = renderComponentWithRouter(<Signin />);
        const menuItems = await findByTestId("main-menu").catch(() => { return null; });
        expect(menuItems).toBe(null);
    });
})

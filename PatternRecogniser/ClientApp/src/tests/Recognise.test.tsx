import '@testing-library/jest-dom';

import { render, screen } from '@testing-library/react';

import RecognisePage from '../pages/Recognise';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};
  
describe("RecognisePanel", () => {
    it("renders recognise panel", () => {
        render(<RecognisePage />);
        expect(screen.getByText("Rozpoznawanie znaku")).toBeInTheDocument();
    });
})

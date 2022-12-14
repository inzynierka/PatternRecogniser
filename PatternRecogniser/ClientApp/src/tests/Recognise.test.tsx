import '@testing-library/jest-dom';

import { render, screen } from '@testing-library/react';
import React from 'react';

import RecognisePage from '../pages/Models/Recognise';

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

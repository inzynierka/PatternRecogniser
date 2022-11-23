import '@testing-library/jest-dom';

import { screen } from '@testing-library/react';
import React from 'react';

import ComparisonLists from '../pages/ComparisonLists';
import { renderComponentWithRouter } from './util';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};
  
describe("Porównywarka", () => {
    it("renders porównywarka panel", () => {
        renderComponentWithRouter(<ComparisonLists />);
        expect(screen.getByText("Porównywarka")).toBeInTheDocument();
    });
})

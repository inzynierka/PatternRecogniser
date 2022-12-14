import '@testing-library/jest-dom';

import { fireEvent, screen } from '@testing-library/react';
import React from 'react';

import MyModelsPage from '../pages/Models/MyModels';
import { renderComponentWithRouter } from './util';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};
  
describe("MyModelsPanel", () => {
    it("renders my models panel", () => {
        renderComponentWithRouter(<MyModelsPage />);
        expect(screen.getByText("Moje modele")).toBeInTheDocument();
    });

    it("displays add model button", () => {
        renderComponentWithRouter(<MyModelsPage />);
        expect(screen.getByText("Dodaj nowy model")).toBeInTheDocument();
    });
    it("displays model list", () => {
        renderComponentWithRouter(<MyModelsPage />);
        expect(screen.getByTestId("model-list-card")).toBeInTheDocument();
    });
    it("displays search bar", () => {
        renderComponentWithRouter(<MyModelsPage />);
        expect(screen.getByTestId("search-input")).toBeInTheDocument();
    });
    it("allows typing in search bar", () => {
        renderComponentWithRouter(<MyModelsPage />);
        const searchInput = screen.getByTestId("search-input")

        expect(searchInput).toHaveValue("");
        fireEvent.change(searchInput, { target: { value: "abc" } });
        expect(searchInput).toHaveValue("abc");
    });
})

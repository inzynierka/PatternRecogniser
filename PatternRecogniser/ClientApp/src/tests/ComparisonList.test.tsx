import '@testing-library/jest-dom';

import { render, screen } from '@testing-library/react';
import React from 'react';

import ComparisonList from '../pages/ComparisonList';
import { ComparisonListType } from '../types/ComparisonType';


window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};

let modelsList : ComparisonListType = {
    name: "models-list",
    elementNum: 6,
}
let signsList : ComparisonListType = {
    name: "signs-list",
    elementNum: 8,
    usedModel: "models-list"
}
  
describe("ComparisonList", () => {
    it("renders model list element", () => {
        render(<ComparisonList list={modelsList} />);

        expect(screen.getByText(modelsList.name)).toBeInTheDocument();
    });
    it("renders sign list element", () => {
        render(<ComparisonList list={signsList} />);

        expect(screen.getByText(signsList.name)).toBeInTheDocument();
    });

    it("renders model list element with correct buttons", () => {
        render(<ComparisonList list={modelsList} />);

        expect(screen.getByText("Szczegóły")).toBeInTheDocument();
        expect(screen.getByTestId("delete-button")).toBeInTheDocument();
    });
    it("renders sign list element with correct buttons", () => {
        render(<ComparisonList list={signsList} />);

        expect(screen.getByText("Szczegóły")).toBeInTheDocument();
        expect(screen.getByTestId("delete-button")).toBeInTheDocument();
    });
})

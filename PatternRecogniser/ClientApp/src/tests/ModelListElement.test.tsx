import '@testing-library/jest-dom';

import { queryByTestId, render, screen } from '@testing-library/react';
import React from 'react';

import ModelListElement from '../pages/ModelListElement';
import { ModelType } from '../types/ModelType';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};
  
const mockedModel : ModelType = {
    name : 'mocked model name',
    patternNum : 17
}

describe("ModelListPanel", () => {
    it("renders model list panel", () => {
        render(<ModelListElement model={mockedModel} />);
        expect(screen.getByText(mockedModel.name)).toBeInTheDocument();
    });
    it("renders model list panel when adding to list", () => {
        render(<ModelListElement model={mockedModel} addingToList={true}/>);
        expect(screen.getByText(mockedModel.name)).toBeInTheDocument();
    });

    it("renders model list panel with correct buttons", () => {
        render(<ModelListElement model={mockedModel} />);
        expect(screen.getByText("Szczegóły")).toBeInTheDocument();
        expect(screen.getByTestId("delete-button")).toBeInTheDocument();

        expect(queryByTestId(document.body, "add-button")).toBeNull();
    });
    it("renders model list panel when adding to list with correct buttons", () => {
        render(<ModelListElement model={mockedModel} addingToList={true}/>);
        expect(screen.getByText("Dodaj do listy")).toBeInTheDocument();

        expect(queryByTestId(document.body, "details-button")).toBeNull();
        expect(queryByTestId(document.body, "delete-button")).toBeNull();
    });
})

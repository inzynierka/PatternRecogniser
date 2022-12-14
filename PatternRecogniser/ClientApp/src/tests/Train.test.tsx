import '@testing-library/jest-dom';

import { render, screen } from '@testing-library/react';
import React from 'react';

import TrainPage from '../pages/Models/Train';

window.matchMedia = window.matchMedia || function() {
    return {
        matches: false,
        addListener: function() {},
        removeListener: function() {}
    };
};
  
describe("TrainPanel", () => {
    it("renders train panel", () => {
        render(<TrainPage />);
        expect(screen.getByText("Trenowanie modelu")).toBeInTheDocument();
    });

    it("displays train form", async () => {
        const { findByTestId } = render(<TrainPage />);

        const trainForm = await findByTestId("train-form");
        const distributionTypeSelect = await findByTestId("distribution-type-select");
        const customCheckbox = await findByTestId("custom-checkbox");
        const chooseFileButton = await findByTestId("choose-file-button");
        const trainButton = await findByTestId("train-button");

        expect(trainForm).toBeInTheDocument();
        expect(distributionTypeSelect).toBeInTheDocument();
        expect(customCheckbox).toBeInTheDocument();
        expect(chooseFileButton).toBeInTheDocument();
        expect(trainButton).toBeInTheDocument();
    });

    it("train/test option is selected by default", async () => {
        const { findByTestId } = render(<TrainPage />);

        const distributionTypeSelect = await findByTestId("distribution-type-select");

        expect(distributionTypeSelect.firstChild?.textContent).toBe("podział train/test");
    });

    it("train button is disabled when no file is sent", async () => {
        const { findByTestId } = render(<TrainPage />);

        const trainButton = await findByTestId("train-button");

        expect(trainButton).toBeDisabled();
    });
    it("send button is disabled when no file is sent", async () => {
        const { findByTestId } = render(<TrainPage />);

        const sendButton = await findByTestId("send-button");

        expect(sendButton).toBeDisabled();
    });
})

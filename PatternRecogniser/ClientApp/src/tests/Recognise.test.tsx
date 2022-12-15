import '@testing-library/jest-dom';

import { render, screen } from '@testing-library/react';
import { RcFile } from 'antd/lib/upload';
import React from 'react';
import { ApiService, ILogIn, LogIn } from '../generated/ApiService';

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

describe("RecogniseIntegrationTests", () => {
    const mockedLoginData : ILogIn = {
        login: "TestAccount",
        password: "Abc123!@#"
    }
    const mockedModelName = "TestModel";
    const mockedFile = new File([""], "testFile.zip", {type: "text/plain"});

   const logIn = () => {
        const apiService = new ApiService();
        return apiService.logIn(new LogIn(mockedLoginData))
        .then(response => response.json())
        .then(
            (data) => {
                localStorage.setItem('token', data.tokens.accessToken);
                return Promise.resolve();
            },
            () => {
                return Promise.reject();
            }
        )
   }

   it("renders data when loggedIn", () => {
        logIn().then(() => {
            render(<RecognisePage />);
            expect(screen.getByText("Rozpoznawanie znaku")).toBeInTheDocument();
        });
   });

   it("pattern recognising works (receives a valid response)", () => {
        logIn().then(() => {
            let apiService = new ApiService();

            apiService.patternRecognition(mockedModelName, mockedFile as RcFile)
            .then(response => response.json())
            .then(
                (data) => {
                    expect(data).not.toBeNull();
                },
                () => {
                    expect(false).toBe(true)
                }
        )
        });
   });
});

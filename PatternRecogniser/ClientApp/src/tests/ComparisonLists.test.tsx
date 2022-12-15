import '@testing-library/jest-dom';

import { screen } from '@testing-library/react';
import React from 'react';
import { ApiService, ILogIn, LogIn } from '../generated/ApiService';

import ComparisonLists from '../pages/ComparisonLists/ComparisonLists';
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

describe("PorównywarkaIntegrationTests", () => {
    const mockedLoginData : ILogIn = {
        login: "TestAccount",
        password: "Abc123!@#"
    }
    const mockedListName = "Testowa lista";

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
            renderComponentWithRouter(<ComparisonLists />);
            expect(screen.getByText("Moje modele")).toBeInTheDocument();
        })
        .catch(() => {
            expect(false).toBe(true)
        });
    });
    it("receives correct models from api", () => {
        // special testing account has one list only: "Testowa lista"
        logIn()
        .then(() => {
            let apiService = new ApiService();
            apiService.getLists()
                .then(response => response.json())
                .then(
                    (data) => {
                        expect(data).not.toBeNull();
                        expect(data).toHaveLength(1);
                        expect(data[0].name).toBe(mockedListName);
                    },
                    () => {
                        expect(false).toBe(true)
                    }
                )
                .catch(() => {
                    expect(false).toBe(true)
                });
        })
        .catch(() => {
            expect(false).toBe(true)
        });
    });
})
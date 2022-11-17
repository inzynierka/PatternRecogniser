import { Dispatch } from 'react';

export interface GlobalStateInterface {
  persistenceType: string;
  isUserAuthenticated: boolean;
  loggedUser: string;
  token: string;
}

export type ActionType = {
  type: string;
  payload?: any;
};

export type ContextType = {
  globalState: GlobalStateInterface;
  dispatch: Dispatch<ActionType>;
};
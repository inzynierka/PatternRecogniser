import { GlobalStateInterface } from "./Types";

export const initialState: GlobalStateInterface = {
  persistenceType: 'sessionStorage',
  isUserAuthenticated: false,
  loggedUser: '',
  token: ''
};
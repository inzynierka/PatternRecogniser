import { GlobalStateInterface } from "./Types";

export const initialState: GlobalStateInterface = {
  isUserAuthenticated: false,
  loggedUser: '',
  token: '',
};
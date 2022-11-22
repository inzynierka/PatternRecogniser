import { GlobalStateInterface } from './Types';

export const initialState: GlobalStateInterface = {
  persistenceType: 'localStorage',
  isUserAuthenticated: false,
  loggedUser: '',
  token: ''
};
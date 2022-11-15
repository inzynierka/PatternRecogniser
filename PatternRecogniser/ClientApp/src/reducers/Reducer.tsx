import { ActionType } from './Types';
import { initialState }  from './initial';

const Reducer = (state = initialState, action: ActionType): any => {
  switch (action.type) {
    case 'SET_USER':
      return {
        ...state,
        loggedUser: action.payload,
      };
    case 'AUTHENTICATE_USER':
      return {
        ...state,
        isUserAuthenticated: action.payload,
      };
    case 'SET_TOKEN':
      return {
        ...state,
        token: action.payload,
      };
    case 'PURGE_STATE':
      return initialState;
    default:
      return state;
  }
};

export default Reducer;
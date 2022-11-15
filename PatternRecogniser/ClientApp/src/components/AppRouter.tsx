import { useContext } from 'react';
import { Route, Routes } from "react-router-dom";
import NotFound from './NotFound';
import { globalContext } from '../reducers/GlobalStore';
import Login from '../components/Login';
import SignIn from '../components/Signin';


export const AppRouter: React.FC = () => {
  const { globalState } = useContext(globalContext);

  return (
      <Routes>
            { !globalState.isUserAuthenticated && 
              <>
                <Route path='/signin' element={<SignIn />}/>
                <Route path='*' element={<Login />}/> 
              </>  
            }
            <Route path='/login' element={<Login />}/>
            <Route path='/signin' element={<SignIn />}/>
            <Route path='*' element={<NotFound />}/>
      </Routes>
  );
}

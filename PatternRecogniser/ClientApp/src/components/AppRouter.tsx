import { useContext } from 'react';
import { Route, Routes } from 'react-router-dom';

import ComparisonPage from '../pages/ComparisonLists';
import CreateComparisonListPage from '../pages/CreateComparisonList';
import Login from '../pages/Login';
import MyAccountPage from '../pages/MyAccount';
import MyModelsPage from '../pages/MyModels';
import RecognisePage from '../pages/Recognise';
import SignIn from '../pages/Signin';
import TrainPage from '../pages/Train';
import { globalContext } from '../reducers/GlobalStore';
import { Urls } from '../types/Urls';
import NotFound from './NotFound';


export const AppRouter: React.FC = () => {
  const { globalState } = useContext(globalContext);

  return (
    <>
      { !globalState.isUserAuthenticated && 
        <Routes>
          <Route path='/signin' element={<SignIn />}/>  
          <Route path='*' element={<Login />}/>  
        </Routes>    
      }
      {
        globalState.isUserAuthenticated &&
        <Routes>
            <Route path={Urls.LogIn} element={<Login />}/>
            <Route path={Urls.SignIn} element={<SignIn />}/>
            <Route path={Urls.Train} element={<TrainPage />}/>
            <Route path={Urls.Recognise} element={<RecognisePage />}/>
            <Route path={Urls.MyModels} element={<MyModelsPage />}/>
            <Route path='comparison-lists' >
              <Route path='create' element={<CreateComparisonListPage />}/>
              <Route path='' element={<ComparisonPage />}/>
            </Route>
            <Route path='/my-account/:accountName' element={<MyAccountPage />}/>
            <Route path='*' element={<NotFound />}/>
        </Routes>
      }
    </>
  );
}

import { Route, Routes } from 'react-router-dom';

import ComparisonPage from '../pages/ComparisonLists/ComparisonLists';
import CreateComparisonListPage from '../pages/ComparisonLists/CreateComparisonList';
import Login from '../pages/Account/Login';
import MyAccountPage from '../pages/Account/MyAccount';
import MyModelsPage from '../pages/Models/MyModels';
import RecognisePage from '../pages/Models/Recognise';
import SignIn from '../pages/Account/Signin';
import TrainPage from '../pages/Models/Train';
import { Urls } from '../types/Urls';
import NotFound from './NotFound';


export const AppRouter: React.FC = () => {
  let isUserAuthenticated = localStorage.getItem('token') !== null;

  return (
    <>
      { !isUserAuthenticated && 
        <Routes>
          <Route path='/signin' element={<SignIn />}/>  
          <Route path='*' element={<Login />}/>  
        </Routes>    
      }
      {
        isUserAuthenticated &&
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
            <Route path='' element={<TrainPage />}/>
            <Route path='*' element={<NotFound />}/>
        </Routes>
      }
    </>
  );
}

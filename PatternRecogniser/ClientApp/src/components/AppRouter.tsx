import { useContext } from 'react';
import { Route, Routes } from "react-router-dom";
import NotFound from './NotFound';
import { globalContext } from '../reducers/GlobalStore';
import Login from '../pages/Login';
import SignIn from '../pages/Signin';
import TrainPage from '../pages/Train';
import RecognisePage from '../pages/Recognise';
import MyModelsPage from '../pages/MyModels';
import ComparisonPage from '../pages/ComparisonLists';


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
            <Route path='/train' element={<TrainPage />}/>
            <Route path='/recognise' element={<RecognisePage />}/>
            <Route path='/my-models' element={<MyModelsPage />}/>
            <Route path='/comparison-lists' element={<ComparisonPage />}/>
            <Route path='*' element={<NotFound />}/>
      </Routes>
  );
}

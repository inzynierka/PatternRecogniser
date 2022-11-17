import { Component } from 'react';
import { AppLayout } from './components/AppLayout';
import { AppRouter } from './components/AppRouter'
import './custom.css'
import { GlobalStore } from './reducers/GlobalStore';
import { Beforeunload } from 'react-beforeunload';

const removeApplicationData = () => {
    if (window) {
        localStorage.clear();
    }
};

export default class App extends Component {
    static displayName = App.name;
  
    render () {
        return (
            <Beforeunload onBeforeunload={removeApplicationData}>
                <GlobalStore>
                    <AppLayout>
                        <AppRouter />
                    </AppLayout>
                </GlobalStore>
            </Beforeunload>
        );
    }
}

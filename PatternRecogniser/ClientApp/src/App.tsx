import { Component } from 'react';
import { AppLayout } from './components/AppLayout';
import { AppRouter } from './components/AppRouter'
import './custom.css'
import { GlobalStore } from './reducers/GlobalStore';

export default class App extends Component {
    static displayName = App.name;
  
    render () {
        return (
            <GlobalStore>
                <AppLayout>
                    <AppRouter />
                </AppLayout>
            </GlobalStore>
        );
    }
}

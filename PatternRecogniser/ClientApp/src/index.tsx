import ReactDOM from 'react-dom';
import App from './App';
import { BrowserRouter } from 'react-router-dom';
//import { BookingsControllerApi, Configuration, ConfigurationParameters, JwtAuthenticationControllerApi, JwtUserControllerApi } from './app/api';

// const config = new Configuration({
//     basePath: "http://localhost:8090",
//     apiKey: "Bearer eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJqYW5rb0B0ZXN0LnBsIiwiZXhwIjoxNjQyODkyMzI5LCJpYXQiOjE2NDI4MDU5Mjl9.4cXyaSXmK4bJZCFmVFECINkaw3SHIcryQPNGHoVKNQhvsFXoDHmXdHvYJdnWxwoQEZTnytiswuGH-v4DQVisrw"
// });

// var client = new BookingsControllerApi(config);
// client.getUserBookingsUsingGET()
//     .then(response => console.log(response))
//     .catch(error => console.log(error));


ReactDOM.render(
    <BrowserRouter basename="/">
        <App />
    </BrowserRouter>,
    document.getElementById('root')
);


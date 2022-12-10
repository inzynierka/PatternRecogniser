import 'antd/dist/antd.min.css';

import { LockOutlined, UserOutlined, LoadingOutlined, Loading3QuartersOutlined } from '@ant-design/icons';
import { Alert, Button, Form, Input, message, Row, Typography } from 'antd';
import { useState } from 'react';
import { useNavigate } from 'react-router';

import { Urls } from '../types/Urls';
import useWindowDimensions from '../UseWindowDimensions';

const { Title } = Typography;

export default function Login() {
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const [userNotFound, setUserNotFound] = useState(false);
    const isOrientationVertical  = useWindowDimensions();
    const [waiting, setWaiting] = useState(false);

    const successfullLogIn = (user : any, accessToken : string, refreshToken : string) => {
        localStorage.setItem('token', 'Bearer ' + accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('userId', user.login);
        localStorage.setItem('email', user.email);

        message.success('Logged in succesfully!');
        setUserNotFound(false);
        setWaiting(true);
        navigate(Urls.Train, { replace: true });
        window.location.reload();
    }

    const login = async (user : any) => {
        setWaiting(true);
        let url = 'https://localhost:44314/LogIn'; 

        fetch(url, {
            method: 'POST',
            headers: {
                'accept': '*/*', 
                'Content-Type': 'application/json-patch+json',
                'mode': 'no-cors'
            },
            body: JSON.stringify({
                login: user.login,
                password: user.password
            })
        })
            .then(response => response.json())
            .then(
                (data) => {
                    setWaiting(false);
                    console.log("zalogowano pomyślnie, token: " + data.accessToken + " refresh token: " + data.refreshToken);
                    successfullLogIn(user, data.accessToken, data.refreshToken);
                },
                (error) => {
                    setUserNotFound(true);
                    setWaiting(false);
                    console.log(user);
                    console.error(error);
                    //successfullLogIn(user, '', '')
                    return;
                }
            )
    }

    const loginHandler = (user : any) => {
        login(user);
    }

    const signInHandler = () => {
        navigate(Urls.SignIn, {replace: true});
    }

    return (
        <div>
            <Row justify="space-around" align="middle" style={{minHeight: "81vh" }}>
                {
                    !waiting &&
                    <Form 
                        layout='vertical'
                        form={form}
                        name="normal_login"
                        className="login-form"
                        data-testid="login-form"
                        onFinish={loginHandler}
                    >
                        <Title>Logowanie</Title>

                        <Form.Item name="login" label="Login"
                            rules={[
                                {
                                    required: true,
                                    message: 'Login nie może być pusty!',
                                },
                            ]}
                        >
                            <Input name="login-input" data-testid="login-input" prefix={<UserOutlined className="site-form-item-icon" />} placeholder="Login" size="large" style={{ width: isOrientationVertical ? "30vw" : "50vw" }}/>
                        </Form.Item>

                        <Form.Item label="Hasło" name="password" hasFeedback
                            rules={[ {required: true, message: 'Proszę wprowadzić hasło!',} ]}
                        >
                            <Input.Password 
                                prefix={<LockOutlined className="site-form-item-icon" />}
                                type="password"
                                placeholder="Hasło"
                                size="large"
                                name="password-input"
                                data-testid="password-input"
                                style={{ width: isOrientationVertical ? "30vw" : "50vw" }}
                            />
                        </Form.Item>


                        <Form.Item>
                            <Row justify="space-between" style={{ width: isOrientationVertical ? "30vw" : "50vw" }}>
                                <Button type="default" data-testid="signin-button" className="login-form-button" onClick={() => signInHandler()} style={{width: isOrientationVertical ? "13vw" : "23vw" }}>Zarejestruj</Button>
                                <Button type="primary" data-testid="login-button" htmlType="submit" className="login-form-button" style={{width: isOrientationVertical ? "13vw" : "23vw" }}>Zaloguj</Button>
                            </Row>
                        </Form.Item>
                        

                        {
                            userNotFound && 
                            <Alert
                            message="Logowanie nie powiodło się."
                            description="Sprawdź czy wprowadzone dane są poprawne."
                            type="error"
                            showIcon
                            />
                        }
                            
                    </Form>
                }
                {
                    waiting &&
                    <div>
                        <Title>Logowanie...</Title>
                        <br />
                        <Loading3QuartersOutlined style={{ fontSize: '15em', marginTop: "40px" }} spin={true} />
                    </div>
                }
            </Row>
        </div>
    );

}

import 'antd/dist/antd.min.css';

import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { Alert, Button, Form, Input, message, Row, Typography } from 'antd';
import { useContext } from 'react';
import { useState } from 'react';
import { useNavigate } from 'react-router';

import { globalContext } from '../reducers/GlobalStore';
import { Urls } from '../types/Urls';
import useWindowDimensions from '../UseWindowDimensions';

const { Title } = Typography;

export default function Login() {
    const [form] = Form.useForm();
    const { dispatch } = useContext(globalContext);
    const navigate = useNavigate();
    const [userNotFound, setUserNotFound] = useState(false);
    const isOrientationVertical  = useWindowDimensions();

    const successfullLogIn = (user : any, token : string) => {
        setUserNotFound(false);
        dispatch({ type: 'AUTHENTICATE_USER', payload: true });
        dispatch({ type: 'SET_TOKEN', payload: token });
        dispatch({ type: 'SET_USER', payload: user.login });
        message.success('Logged in succesfully!');
        navigate(Urls.Train, {replace: true});
    }
    const demoLogin = async (user : any) => {
        return (user.login === "admin" && user.password === "admin") 
    }
    const loginHandler = (user : any) => {
        demoLogin(user).then((result) => {
            if(result)
                successfullLogIn(user,"Bearer ");
            else {
                setUserNotFound(true);
                console.log(user);
                console.error("User not found")
            }
        });
    }

    const signInHandler = () => {
        navigate(Urls.SignIn, {replace: true});
    }

    return (
        <div>
            <Row justify="space-around" align="middle" style={{minHeight: "81vh" }}>
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
                        message="Niepoprawne dane"
                        description="Logowanie nie powiodło się. Sprawdź czy wprowadzone dane są poprawne."
                        type="error"
                        showIcon
                        />
                    }
                        
                </Form>
            </Row>
        </div>
    );

}

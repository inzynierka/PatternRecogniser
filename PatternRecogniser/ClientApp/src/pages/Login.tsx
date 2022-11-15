import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { Row, Button, Input, Form, message, Alert, Col, Space } from 'antd';
import { useContext, useEffect } from 'react';
import {useNavigate} from 'react-router-dom';
import { useState } from 'react';
import 'antd/dist/antd.min.css';
import { globalContext } from '../reducers/GlobalStore';
import { Typography } from 'antd';

const { Title } = Typography;

interface Props {
}

export default function Login(props : Props) {
    const [form] = Form.useForm();
    const { dispatch } = useContext(globalContext);
    const navigate = useNavigate();
    const [userNotFound, setUserNotFound] = useState(false);

    const successfullLogIn = (user : any, token : string) => {
        setUserNotFound(false);
        dispatch({ type: 'AUTHENTICATE_USER', payload: true });
        dispatch({ type: 'SET_TOKEN', payload: token });
        dispatch({ type: 'SET_USER', payload: user.login });
        message.success('Logged in succesfully!');
        navigate('/train', {replace: true});
    }

    const demoLogin = (user : any) => {
        return (user.login === "admin" && user.password === "admin") 
    }
    const loginHandler = (user : any) => {
        if(demoLogin(user))
            successfullLogIn(user, "Bearer ");
        else {
            setUserNotFound(true);
            console.log(user);
            console.error("User not found")
        }
    }

    const signInHandler = () => {
        navigate('/signin', {replace: true});
    }

    return (
        <div>
            <Row justify="space-around" align="middle" style={{minHeight: "81vh" }}>
                <Form 
                    layout='vertical'
                    form={form}
                    name="normal_login"
                    className="login-form"
                    onFinish={loginHandler}
                >
                    <Title>Logowanie</Title>

                    <Form.Item name="login" label="Login"
                        rules={[
                            {
                                required: true,
                                message: 'Login nie może być pusty!',
                            },
                    ]}>
                        <Input prefix={<UserOutlined className="site-form-item-icon" />} placeholder="Login" size="large" style={{ width: "60vh" }}/>
                    </Form.Item>

                    <Form.Item label="Hasło" name="password" hasFeedback
                        rules={[ {required: true, message: 'Proszę wprowadzić hasło!',} ]}
                    >
                        <Input.Password 
                            prefix={<LockOutlined className="site-form-item-icon" />}
                            type="password"
                            placeholder="Hasło"
                            size="large"
                            style={{ width: "60vh" }}
                        />
                    </Form.Item>


                    <Form.Item>
                        <Row justify="space-between" style={{ width: "60vh" }}>
                            <Button type="default" className="login-form-button" onClick={() => signInHandler()} style={{width: "29vh" }}>Zarejestruj</Button>
                            <Button type="primary" htmlType="submit" className="login-form-button" style={{width: "29vh" }}>Zaloguj</Button>
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

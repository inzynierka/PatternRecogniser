import 'antd/dist/antd.min.css';

import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { Alert, Button, Form, Input, message, Row, Typography } from 'antd';
import { useContext } from 'react';
import { useState } from 'react';
import PasswordChecklist from 'react-password-checklist';
import { useNavigate } from 'react-router-dom';

import { Urls } from '../types/Urls';
import useWindowDimensions from '../UseWindowDimensions';

const { Title } = Typography;

interface Props {
}

export default function SignIn(props : Props) {
    const [password, setPassword] = useState("")
	const [passwordAgain, setPasswordAgain] = useState("")
    const [correctPassword, setCorrectPassword] = useState(true);
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const isOrientationVertical  = useWindowDimensions();

    const successfullSignIn = (user : any, token : string) => {
        localStorage.setItem('token', token);
        localStorage.setItem('userId', user.login);
        localStorage.setItem('email', user.email);
        message.success('Logged in succesfully!');
        navigate(Urls.Train, {replace: true});
    }

    const signinHandler = (user : any) => {
        successfullSignIn(user,"Bearer ");
        navigate(Urls.Train, {replace: true});
    }

    const cancelHandler = () => {
        navigate(Urls.LogIn, {replace: true});
    }

    const passwordValidate = (password : string) => {
        const strongRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{8,})");
        const result = strongRegex.test(password);
        setCorrectPassword(result)
        return result;
    }  

    const passwordChecklist = () => {
        return (
            <PasswordChecklist
                rules={["minLength","specialChar","number","capital","match"]}
                minLength={8}
                value={password}
                valueAgain={passwordAgain}
                messages={{
                    minLength: "Hasło zawiera więcej niż 8 znaków.",
                    specialChar: "Hasło zawiera znaki specjalne.",
                    number: "Hasło zawiera cyfrę.",
                    capital: "Hasło zawiera wielką literę.",
                    match: "Hasła są takie same.",
                }}
            />
        )
    }

    return (
        <div>
            <Row justify="space-around" align="middle" style={{minHeight: "81vh" }}>
                <Form 
                    layout='vertical'
                    form={form}
                    name="normal-signin"
                    className="signin-form"
                    data-testid="signin-form"
                    onFinish={signinHandler}
                >
                    <Title>Rejestracja</Title>

                    <Form.Item name="email" label="Adres e-mail"
                        rules={[
                            {
                                required: true,
                                message: 'Adres e-mail nie może być pusty.',
                            },
                            {
                                type: 'email',
                                message: 'Proszę wprowadzić prawidłowy adres e-mail.'
                            },
                    ]}>
                        <Input data-testid="email-input" name="email-input" prefix={<UserOutlined className="site-form-item-icon" />} placeholder="Adres e-mail" size="large" style={{ width: isOrientationVertical ? "30vw" : "50vw" }}/>
                    </Form.Item>

                    <Form.Item name="login" label="Login"
                        rules={[
                            {
                                required: true,
                                message: 'Login nie może być pusty.',
                            },
                    ]}>
                        <Input data-testid="login-input" name="login-input" prefix={<UserOutlined className="site-form-item-icon" />} placeholder="Login" size="large" style={{ width: isOrientationVertical ? "30vw" : "50vw" }}/>
                    </Form.Item>

                    <Form.Item label="Hasło" name="password" hasFeedback
                        rules={[ 
                            {required: true, message: 'Proszę wprowadzić hasło.',},
                            ( ) => ({
                                validator(_, value) {
                                if (!value || passwordValidate(value)) {
                                    return Promise.resolve();
                                }
                                return Promise.reject(new Error('Za słabe hasło.'));
                                },
                            }),
                        ]}
                       
                    >
                        <Input.Password 
                            prefix={<LockOutlined className="site-form-item-icon" />}
                            type="password"
                            placeholder="Hasło"
                            size="large"
                            data-testid="password-input"
                            name="password-input"
                            onChange={e => setPassword(e.target.value)}
                            maxLength={32}
                            style={{ width: isOrientationVertical ? "30vw" : "50vw" }}
                        />
                    </Form.Item>

                    <Form.Item
                        name="confirm"
                        label="Powtórz hasło"
                        dependencies={['password']}
                        hasFeedback
                        rules={[
                        {
                            required: true,
                            message: 'Proszę powtórzyć hasło.',
                        },
                        ({ getFieldValue }) => ({
                            validator(_, value) {
                            if (!value || getFieldValue('password') === value) {
                                return Promise.resolve();
                            }
                            return Promise.reject(new Error('Wprowadzone hasła nie są takie same.'));
                            },
                        }),
                        ]}
                    >
                        <Input.Password 
                            prefix={<LockOutlined className="site-form-item-icon" />}
                            type="password"
                            placeholder="Powtórz hasło"
                            onChange={e => setPasswordAgain(e.target.value)}
                            size="large"
                            data-testid="password-confirm-input"
                            name="password-confirm-input"
                            maxLength={32}
                            style={{ width: isOrientationVertical ? "30vw" : "50vw" }}
                        />
                    </Form.Item>

                    {
                        !correctPassword && 
                        <Alert
                        message="Niepoprawne hasło"
                        description={passwordChecklist()}
                        type="error"
                        showIcon
                        />
                    }

                    <br />

                    <Form.Item>
                        <Row justify="space-between" style={{ width: isOrientationVertical ? "30vw" : "50vw" }}>
                            <Button data-testid="cancel-button" type="default" className="login-form-button" onClick={() => cancelHandler()} style={{width: isOrientationVertical ? "13vw" : "23vw"}}>Anuluj</Button>
                            <Button data-testid="signin-button" type="primary" htmlType="submit" className="login-form-button" style={{width: isOrientationVertical ? "13vw" : "23vw"}}>Zarejestruj</Button>
                        </Row>
                    </Form.Item>
                                               
                </Form>
            </Row>
        </div>
    );

}

import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { Row, Button, Input, Form, message, Alert } from 'antd';
import { useContext } from 'react';
import {useNavigate} from 'react-router-dom';
import { useState } from 'react';
import 'antd/dist/antd.min.css';
import { globalContext } from '../reducers/GlobalStore';
import { Typography } from 'antd';
import PasswordChecklist from "react-password-checklist"

const { Title } = Typography;

interface Props {
}

export default function SignIn(props : Props) {
    const [password, setPassword] = useState("")
	const [passwordAgain, setPasswordAgain] = useState("")
    const [correctPassword, setCorrectPassword] = useState(true);
    const [form] = Form.useForm();
    const { dispatch } = useContext(globalContext);
    const navigate = useNavigate();

    const successfullSignIn = (user : any, token : string) => {
        dispatch({ type: 'AUTHENTICATE_USER', payload: true });
        dispatch({ type: 'SET_TOKEN', payload: token });
        dispatch({ type: 'SET_USER', payload: user.login });
        message.success('Logged in succesfully!');
        navigate('/train', {replace: true});
    }

    const signinHandler = (user : any) => {
        successfullSignIn(user,"Bearer ");
        navigate('/train', {replace: true});
    }

    const cancelHandler = () => {
        navigate('/login', {replace: true});
    }

    const passwordValidate = (password : string) => {
        const strongRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{8,})");
        const result = strongRegex.test(password);
        setCorrectPassword(result)
        return result;
    }  

    const testowanafunkcja = () => {
        return (
            <PasswordChecklist
                rules={["minLength","specialChar","number","capital","match"]}
                minLength={8}
                value={password}
                valueAgain={passwordAgain}
                messages={{
                    minLength: "Hasło zawiera więcej niż 8 znaków.",
                    specialChar: "Hasło zawiera specjalne znaki.",
                    number: "Hasło zawiera cyfrę.",
                    capital: "Hasło zawiera wielka literę.",
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
                    name="normal_signin"
                    className="signin-form"
                    onFinish={signinHandler}
                >
                    <Title>Rejestracja</Title>

                    <Form.Item name="email" label="Adres e-mail"
                        rules={[
                            {
                                required: true,
                                message: 'Adres e-mail nie może być pusty!!',
                            },
                            {
                                type: 'email',
                                message: 'Proszę wprowadzić prawidłowy adres e-mail!'
                            },
                    ]}>
                        <Input prefix={<UserOutlined className="site-form-item-icon" />} placeholder="Adres e-mail" size="large" style={{ width: "60vh" }}/>
                    </Form.Item>

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
                        rules={[ 
                            {required: true, message: 'Proszę wprowadzić hasło!',},
                            ( ) => ({
                                validator(_, value) {
                                if (!value || passwordValidate(value)) {
                                    return Promise.resolve();
                                }
                                return Promise.reject(new Error('Za słabe hasło!'));
                                },
                            }),
                        ]}
                       
                    >
                        <Input.Password 
                            prefix={<LockOutlined className="site-form-item-icon" />}
                            type="password"
                            placeholder="Hasło"
                            size="large"
                            onChange={e => setPassword(e.target.value)}
                            style={{ width: "60vh" }}
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
                            message: 'Proszę powtórzyć hasło!',
                        },
                        ({ getFieldValue }) => ({
                            validator(_, value) {
                            if (!value || getFieldValue('password') === value) {
                                return Promise.resolve();
                            }
                            return Promise.reject(new Error('Wprowadzone hasła nie są takie same!'));
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
                            style={{ width: "60vh" }}
                        />
                    </Form.Item>

                    {
                        !correctPassword && 
                        <Alert
                        message="Niepoprawne hasło"
                        description={testowanafunkcja()}
                        type="error"
                        showIcon
                        />
                    }

                    

                    <br />

                    <Form.Item>
                        <Row justify="space-between" style={{ width: "60vh" }}>
                            <Button type="default" className="login-form-button" onClick={() => cancelHandler()} style={{width: "29vh" }}>Anuluj</Button>
                            <Button type="primary" htmlType="submit" className="login-form-button" style={{width: "29vh" }}>Zarejestruj</Button>
                        </Row>
                    </Form.Item>
                                               
                </Form>
            </Row>
        </div>
    );

}

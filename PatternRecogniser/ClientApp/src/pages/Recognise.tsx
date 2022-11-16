import {Typography, Form, Card } from "antd"

import 'antd/dist/antd.min.css';
import { Row, Col } from "antd";

const { Title } = Typography;


const RecognisePage = () => {
    const [form] = Form.useForm();

    const onFinish = (values: any) => {
        console.log('Received values of form: ', values);
    };

    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content">
                        <Row justify="space-around" align="middle">
                            <Title>Rozpoznawanie znaku</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Card bordered={true} style={{width: "70vh", boxShadow: '0 3px 10px rgb(0 0 0 / 0.2)' }}>
                                <Row justify="space-around" align="middle">
                                    <Form 
                                        layout='horizontal'
                                        form={form}
                                        name="train_options"
                                        className="train-form"
                                        onFinish={onFinish}
                                    >                                    
                                        
                                    {/* POLA FORMULARZA */}

                                    </Form>
                                </Row>
                            </Card>
                        </Row>
                    </div>      
                </Col>
            </Row>
        </div>
    );
}

export default RecognisePage;
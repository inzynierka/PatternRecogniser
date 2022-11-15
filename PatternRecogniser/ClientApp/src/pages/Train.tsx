import {Card, Typography, Form, Space, Tooltip, Input, Select, Checkbox, InputNumber, Upload, Button, UploadProps, message } from "antd"
import { LoadingOutlined } from '@ant-design/icons';

import React, { useContext, useEffect, useState } from 'react';
import 'antd/dist/antd.min.css';
import { Row, Col } from "antd";
import { globalContext } from "../reducers/GlobalStore";
import { QuestionCircleOutlined, UploadOutlined } from '@ant-design/icons';
import { RcFile, UploadFile } from "antd/lib/upload/interface";

const { Title } = Typography;


const TrainPage = () => {
    const [form] = Form.useForm();
    const { globalState } = useContext(globalContext);
    const [loading, setLoading] = useState(false);
    const [selectedDistributionType, setSelectedDistributionType] = useState("train/test");
    const [train, setTrain] = useState(80);
    const [test, setTest] = useState(20);
    const [emptyfile, _] = useState<UploadFile>();
    const [file, setFile] = useState<UploadFile>();
    const [uploading, setUploading] = useState(false);

    const [customTrainTestValue, setCustomTrainTestValue] = useState(false);;

    const onFinish = (values: any) => {
        console.log('Received values of form: ', values);
    };

    const distributionTypeChanged = (type : string) => {
        setSelectedDistributionType(type);
    }

    const trainChanged = (value : number) => {
        setTrain(value);
        setTest(100 - value);
    }
    const testChanged = (value : number) => {
        setTest(value);
        setTrain(100 - value);
    }

    const handleUpload = () => {
        const formData = new FormData();
        formData.append('file', file as RcFile);
        setUploading(true);
        // You can use any AJAX library you like
        fetch('https://www.mocky.io/v2/5cc8019d300000980a055e76', {
          method: 'POST',
          body: formData,
        })
          .then(res => res.json())
          .then(() => {
            setFile(emptyfile);
            message.success('upload successfully.');
          })
          .catch(() => {
            message.error('upload failed.');
          })
          .finally(() => {
            setUploading(false);
          });
      };

      const props: UploadProps = {
            onChange: info => {
            console.log(info.file);
            },
            onRemove: file => {
                setFile(emptyfile);
            },
            beforeUpload: file => {
                setFile(file);
                return false;
            },
        };

    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content">
                        <Row justify="space-around" align="middle">
                            <Title>Trenowanie modelu</Title>
                        </Row>
                        <Row justify="space-around" align="middle">
                                <Form 
                                    layout='horizontal'
                                    form={form}
                                    name="train_options"
                                    className="train-form"
                                    onFinish={onFinish}
                                >
                                
                                <Form.Item label="Sposób podziału danych: " style={{width: "50vh" }}>
                                    <Space>
                                        <Form.Item
                                            name="distributionType"
                                            noStyle
                                            rules={[{ required: true, message: 'Pole jest wymagane' }]}
                                        >
                                            <Select style={{width: "29vh" }} onChange={distributionTypeChanged}>
                                                <Select.Option value="trainTest">podział train/test</Select.Option>
                                                <Select.Option value="crossValidation">walidacja krzyżowa</Select.Option>
                                            </Select>
                                        </Form.Item>
                                        <Tooltip title="Tu wyświetla się instrukcja dla użytkownika.">
                                            <Typography.Link href="#API"><QuestionCircleOutlined /></Typography.Link>
                                        </Tooltip>
                                    </Space>
                                </Form.Item>

                                {
                                    selectedDistributionType.includes("trainTest") ? 
                                    <Form.Item style={{width: "50vh" }}>
                                        <Row justify="space-around" align="middle">
                                            <Form.Item label="Podział ręczny">
                                                <Checkbox value="customTrainTest" checked={customTrainTestValue} onChange={() => {setCustomTrainTestValue(!customTrainTestValue)}}/>
                                            </Form.Item>

                                            <Form.Item label="Train">
                                                <InputNumber min={1} max={99} onChange={trainChanged} value={train} disabled={!customTrainTestValue}/> %
                                            </Form.Item>

                                            <Form.Item label="Test">
                                                <InputNumber min={1} max={99} onChange={testChanged} value={test} disabled={!customTrainTestValue}/> %
                                            </Form.Item>

                                            <Form.Item>
                                                <Tooltip title="Tu wyświetla się instrukcja dla użytkownika.">
                                                    <Typography.Link href="#API"><QuestionCircleOutlined /></Typography.Link>
                                                </Tooltip>
                                            </Form.Item>
                                        </Row>
                                    </Form.Item>
                                    :
                                    <Form.Item>
                                        <Row justify="space-around" align="middle">
                                            <Form.Item label="Podział ręczny">
                                                    <Checkbox value="customTrainTest" checked={customTrainTestValue} onChange={() => {setCustomTrainTestValue(!customTrainTestValue)}}/>
                                            </Form.Item>
                                            <Form.Item label="Liczba podzbiorów">
                                                <InputNumber min={1} max={50} defaultValue={5} disabled={!customTrainTestValue}/>
                                            </Form.Item>

                                            <Form.Item>
                                                <Tooltip title="Tu wyświetla się instrukcja dla użytkownika.">
                                                    <Typography.Link href="#API"><QuestionCircleOutlined /></Typography.Link>
                                                </Tooltip>
                                            </Form.Item>
                                        </Row>
                                    </Form.Item>
                                }

                                <Row align="middle">
                                    <Form.Item label="Zbiór symboli">
                                        <Upload {...props} maxCount={1} accept='application/zip'>
                                            <Button icon={<UploadOutlined />}>Załaduj plik</Button>
                                        </Upload>
                                        
                                        <Form.Item>
                                            <Button
                                                type="primary"
                                                onClick={handleUpload}
                                                disabled={file === emptyfile}
                                                loading={uploading}
                                                style={{ marginTop: 16 }}
                                            >
                                                {uploading ? 'Wysyłanie' : 'Wyślij'}
                                            </Button>
                                        </Form.Item>
                                    </Form.Item>
                                </Row>
                            </Form>
                        </Row>
                       
                    </div> 
                    <br />            
                </Col>
            </Row>
        </div>
    );
}

export default TrainPage;
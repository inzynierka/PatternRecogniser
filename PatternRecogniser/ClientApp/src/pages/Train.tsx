import 'antd/dist/antd.min.css';

import { QuestionCircleOutlined, UploadOutlined } from '@ant-design/icons';
import {
    Button,
    Card,
    Checkbox,
    Col,
    Form,
    InputNumber,
    message,
    Row,
    Select,
    Space,
    Tooltip,
    Typography,
    Upload,
    UploadProps,
} from 'antd';
import { RcFile, UploadFile } from 'antd/lib/upload/interface';
import React, { useState } from 'react';

import useWindowDimensions from '../UseWindowDimensions';

const { Title } = Typography;


const TrainPage = () => {
    const [form] = Form.useForm();
    const isOrientationVertical  = useWindowDimensions();
    const [selectedDistributionType, setSelectedDistributionType] = useState("trainTest");
    const [train, setTrain] = useState(80);
    const [test, setTest] = useState(20);
    const [emptyfile, ] = useState<UploadFile>();
    const [file, setFile] = useState<UploadFile>();
    const [uploading, setUploading] = useState(false);
    const [uploadSuccessful, setUploadSuccessful] = useState(false);

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
            message.success('Załadowano pomyślnie.');
            setUploadSuccessful(true);
          })
          .catch(() => {
            message.error('Nie udało się przesłać pliku.');
            setFile(emptyfile);
            setUploadSuccessful(false);
          })
          .finally(() => {
            setUploading(false);
          });
      };

    const handleTrain = () => {
        console.log('Train');
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
                    <div className="site-layout-content" style={{paddingBottom: "100px"}}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Trenowanie modelu</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Card bordered={true} style={{width: isOrientationVertical ? "40vw" : "65vw", boxShadow: '0 3px 10px rgb(0 0 0 / 0.2)', paddingTop: '20px' }}>
                                <Row justify="space-around" align="middle">
                                    <Form 
                                        layout='horizontal'
                                        form={form}
                                        name="train_options"
                                        className="train-form"
                                        data-testid="train-form"
                                        onFinish={onFinish}
                                    >                                    
                                        <Form.Item label="Sposób podziału danych: " style={{width: isOrientationVertical ? "15vw" : "40vw" }}>
                                            <Space>
                                                <Form.Item
                                                    name="distributionType"
                                                    noStyle
                                                    rules={[{ required: true, message: 'Pole jest wymagane' }]}
                                                >
                                                    <Select 
                                                        style={{width: "15vw" }} 
                                                        onChange={distributionTypeChanged} 
                                                        defaultValue="trainTest"
                                                        data-testid="distribution-type-select"
                                                    >
                                                        <Select.Option data-testid="trainTest" value="trainTest">podział train/test</Select.Option>
                                                        <Select.Option data-testid="crossValidation" value="crossValidation">walidacja krzyżowa</Select.Option>
                                                    </Select>
                                                </Form.Item>
                                                <Tooltip title="Tu wyświetla się instrukcja dla użytkownika." data-testid="distribution-tooltip">
                                                    <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                                </Tooltip>
                                            </Space>
                                        </Form.Item>

                                        {
                                            selectedDistributionType.includes("trainTest") ? (
                                                isOrientationVertical ?
                                                <Form.Item style={{width: "25vw"}}>
                                                    {/* gdy wybrano podział train/test */}
                                                    <Row justify="space-around" align="middle">
                                                        <Col>
                                                            <Form.Item label="Podział ręczny">
                                                                <Checkbox 
                                                                    value="customTrainTest"
                                                                    checked={customTrainTestValue}
                                                                    onChange={() => {setCustomTrainTestValue(!customTrainTestValue)}}
                                                                    data-testid="custom-checkbox"
                                                                />
                                                            </Form.Item>
                                                        </Col>
                                                       
                                                        <Col style={{marginRight: "-40px" }}>
                                                            <Form.Item label="Train" style={{marginLeft: "-5px" }}>
                                                                <InputNumber 
                                                                    min={1} 
                                                                    max={99} 
                                                                    onChange={trainChanged} 
                                                                    value={train} 
                                                                    disabled={!customTrainTestValue}
                                                                    style={{width: "10vw" }}
                                                                    data-testid="train-input"
                                                                /> %
                                                            </Form.Item>

                                                            <Row>
                                                                <Form.Item label="Test" style={{marginRight: "10px" }}>
                                                                    <InputNumber 
                                                                        min={1} 
                                                                        max={99} 
                                                                        onChange={testChanged} 
                                                                        value={test} 
                                                                        disabled={!customTrainTestValue}
                                                                        style={{width: "10vw" }}
                                                                        data-testid="test-input"
                                                                    /> %
                                                                </Form.Item>
                                                            </Row>
                                                        </Col>

                                                        <Col>
                                                            <Form.Item>
                                                                <Tooltip title="Tu wyświetla się instrukcja dla użytkownika." data-testid="train-test-tooltip">
                                                                    <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                                                </Tooltip>
                                                            </Form.Item>
                                                        </Col>
                                                    </Row>
                                                </Form.Item>
                                            :
                                                <Form.Item style={{width: "35vw" }}>
                                                    {/* Urządzenie mobilne */}
                                                    <Row justify="space-around" align="top">
                                                        <Col>
                                                            <Form.Item label="Podział ręczny">
                                                                <Checkbox 
                                                                    value="customTrainTest" 
                                                                    checked={customTrainTestValue} 
                                                                    onChange={() => {setCustomTrainTestValue(!customTrainTestValue)}}
                                                                    data-testid="custom-checkbox"
                                                                />
                                                            </Form.Item>
                                                        </Col>
                                                       
                                                        <Col>
                                                            <Form.Item label="Train" style={{marginLeft: "-5px" }}>
                                                                <InputNumber
                                                                    min={1}
                                                                    max={99} 
                                                                    onChange={trainChanged} 
                                                                    value={train} 
                                                                    disabled={!customTrainTestValue}
                                                                    data-testid="train-input"
                                                                /> %
                                                            </Form.Item>

                                                            <Row>
                                                                <Form.Item label="Test" style={{marginRight: "10px" }}>
                                                                    <InputNumber 
                                                                        min={1} 
                                                                        max={99} 
                                                                        onChange={testChanged} 
                                                                        value={test} 
                                                                        disabled={!customTrainTestValue}
                                                                        data-testid="test-input"
                                                                    /> %
                                                                </Form.Item>
                                                            </Row>
                                                        </Col>
                                                        <Col>
                                                            <Form.Item>
                                                                <Tooltip title="Tu wyświetla się instrukcja dla użytkownika." data-testid="train-test-tooltip">
                                                                    <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                                                </Tooltip>
                                                            </Form.Item>
                                                        </Col>
                                                    </Row>
                                                </Form.Item>
                                            ):
                                            <Form.Item style={{width: "25vw"}}>
                                                {/* gdy wybrano walidację krzyżową */}
                                                <Row justify="space-around" align="middle">
                                                    <Form.Item label="Podział ręczny">
                                                            <Checkbox 
                                                                value="customTrainTest" 
                                                                checked={customTrainTestValue} 
                                                                onChange={() => {setCustomTrainTestValue(!customTrainTestValue)}}
                                                                data-testid="custom-checkbox"
                                                            />
                                                    </Form.Item>
                                                    <Form.Item label="Liczba podzbiorów">
                                                        <InputNumber 
                                                            min={1} 
                                                            max={50} 
                                                            defaultValue={5} 
                                                            disabled={!customTrainTestValue}
                                                            data-testid="subset-input"
                                                        />
                                                    </Form.Item>

                                                    <Form.Item>
                                                        <Tooltip title="Tu wyświetla się instrukcja dla użytkownika." data-testid="cross-validation-tooltip">
                                                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                                        </Tooltip>
                                                    </Form.Item>
                                                </Row>
                                            </Form.Item>
                                        }

                                        <Row align="middle">
                                            <Form.Item label="Zbiór symboli" style={{width: "22vw" }}>
                                                <Upload {...props} maxCount={1} accept='application/zip'>
                                                    <Button data-testid="choose-file-button" icon={<UploadOutlined />}>Wybierz plik</Button>
                                                </Upload>
                                                
                                                <Form.Item>
                                                    <Button
                                                        type="primary"
                                                        onClick={handleUpload}
                                                        disabled={file === emptyfile}
                                                        loading={uploading}
                                                        data-testid="send-button"
                                                        style={{ marginTop: 16, marginBottom: -20 }}
                                                    >
                                                        {uploading ? 'Wysyłanie' : 'Wyślij'}
                                                    </Button>
                                                </Form.Item>
                                            </Form.Item>
                                        </Row>

                                        <Row justify='end' align="middle">
                                            <Form.Item>
                                                <Button
                                                    type="primary"
                                                    onClick={handleTrain}
                                                    data-testid="train-button"
                                                    disabled={!uploadSuccessful || file === emptyfile}
                                                    style={{ marginTop: '-10' }}
                                                >
                                                    Rozpocznij uczenie
                                                </Button>
                                            </Form.Item>
                                        </Row>
                                    </Form>
                                </Row>
                            </Card>
                        </Row>
                    </div> 
                    <br />            
                </Col>
            </Row>
        </div>
    );
}

export default TrainPage;
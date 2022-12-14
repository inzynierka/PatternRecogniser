import 'antd/dist/antd.min.css';

import { QuestionCircleOutlined, UploadOutlined } from '@ant-design/icons';
import {
    Button,
    Checkbox,
    Col,
    Form,
    Input,
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
import { useState } from 'react';

import { DistributionType } from '../../../generated/ApiService';
import useWindowDimensions from '../../../UseWindowDimensions';

interface TrainFormProps {
    setIsModelBeingTrained: (isModelBeingTrained: boolean) => void;
}

export const TrainForm = (trainFormProps : TrainFormProps) => {
    const isOrientationVertical  = useWindowDimensions();
    const [form] = Form.useForm();
    const [selectedDistributionType, setSelectedDistributionType] = useState(0);
    const [train, setTrain] = useState(80);
    const [test, setTest] = useState(20);
    const [uploading, setUploading] = useState(false);
    const [uploadSuccessful, setUploadSuccessful] = useState(false);
    const [customTrainTestValue, setCustomTrainTestValue] = useState(false);
    const [modelName, setModelName] = useState("");
    const [emptyfile, ] = useState<UploadFile>();
    const [file, setFile] = useState<UploadFile>();

    const distributionTypeChanged = (type : number) => {
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

    const demoTrain = () => {
        console.log("Training model " + modelName);
        setTimeout(() => {
            console.log('Training finished!');
            trainFormProps.setIsModelBeingTrained(false);
        }, 1000);
    }

    const onFinish = (values: any) => {
        trainFormProps.setIsModelBeingTrained(true);
        //let token = localStorage.getItem('token') || "";

        //apiService.trainModel(token, values.modelName, selectedDistributionType || 0, file as RcFile)
        // .then((response) => {
        //     console.log(response);
        // })
        demoTrain();
    };

    const uploadProps: UploadProps = {
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
    const handleUpload = () => {
        const formData = new FormData();
        formData.append('file', file as RcFile);
        setUploading(true);
        // You can use any AJAX library you like
        //apiService.trainModel(token, modelName, selectedDistributionType || 0, file as RcFile)
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

    return (
        <Row justify="space-around" align="middle">
            <Form 
                layout='horizontal'
                form={form}
                name="train_options"
                className="train-form"
                data-testid="train-form"
                onFinish={onFinish}
            >     
                <Form.Item label="Nazwa modelu: " style={{width: isOrientationVertical ? "15vw" : "40vw" }}>
                    <Space>
                        <Form.Item
                            name="modelName"
                            noStyle
                            rules={[{ required: true, message: 'Nazwa modelu nie może być pusta.' }]}
                        >
                            <Input placeholder='Wpisz nazwę modelu' data-testid="model-name-input" style={{width: "18.1vw" }} onChange={(e) => setModelName(e.target.value)}/>
                        </Form.Item>
                        <Tooltip title="Nazwa modelu musi być unikalna." data-testid="name-tooltip">
                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                        </Tooltip>
                    </Space>
                </Form.Item>                               
                <Form.Item label="Sposób podziału danych: " style={{width: isOrientationVertical ? "15vw" : "40vw" }}>
                    <Space>
                        <Form.Item
                            name="distributionType"
                            noStyle
                        >
                            <Select 
                                style={{width: "15vw" }} 
                                onChange={distributionTypeChanged} 
                                defaultValue={DistributionType.TrainTest}
                                data-testid="distribution-type-select"
                            >
                                <Select.Option data-testid={DistributionType.TrainTest} value={DistributionType.TrainTest}>podział train/test</Select.Option>
                                <Select.Option data-testid={DistributionType.CrossValidation} value={DistributionType.CrossValidation}>walidacja krzyżowa</Select.Option>
                            </Select>
                        </Form.Item>
                        <Tooltip title="Tu wyświetla się instrukcja dla użytkownika." data-testid="distribution-tooltip">
                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                        </Tooltip>
                    </Space>
                </Form.Item>

                {
                    selectedDistributionType === DistributionType.TrainTest ? (
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
                        <Upload {...uploadProps} maxCount={1} accept='application/zip'>
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
                            data-testid="train-button"
                            htmlType="submit"
                            disabled={!uploadSuccessful || file === emptyfile}
                            style={{ marginTop: '-10' }}
                        >
                            Rozpocznij uczenie
                        </Button>
                    </Form.Item>
                </Row>
            </Form>
        </Row>
    )
}
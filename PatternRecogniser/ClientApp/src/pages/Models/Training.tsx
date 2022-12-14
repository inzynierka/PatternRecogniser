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

import useWindowDimensions from '../../UseWindowDimensions';
import { DistributionType } from '../../generated/ApiService';

const { Title } = Typography;


const TrainPage = () => {
    const [form] = Form.useForm();
    const isOrientationVertical  = useWindowDimensions();
    const [selectedDistributionType, setSelectedDistributionType] = useState(0);
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
                    </div> 
                    <br />            
                </Col>
            </Row>
        </div>
    );
}

export default TrainPage;
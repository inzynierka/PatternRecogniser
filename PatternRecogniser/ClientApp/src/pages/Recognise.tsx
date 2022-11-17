import {Typography, Form, Card, Space, Select, Tooltip, UploadProps } from "antd"

import 'antd/dist/antd.min.css';
import { Row, Col, message, Upload } from "antd";
import { useState } from "react";
import { QuestionCircleOutlined, InboxOutlined } from '@ant-design/icons';


const { Dragger } = Upload;
const { Title } = Typography;

interface selectOption {
    value : string,
    label : string
}

const Models: selectOption[] = [
    {
        value: 'cyfry_arabskie',
        label: 'Cyfry arabskie'
    },
    {
        value: 'litery_alfabetu',
        label: 'Litery alfabetu'
    },
    {
        value: 'figury_geometryczne',
        label: 'Figury geometryczne'
    }
]

const props: UploadProps = {
    name: 'file',
    multiple: false,
    action: 'https://www.mocky.io/v2/5cc8019d300000980a055e76',
    onChange(info) {
      const { status } = info.file;
      if (status !== 'uploading') {
        console.log(info.file, info.fileList);
      }
      if (status === 'done') {
        message.success(`Załadowano pomyślnie plik ${info.file.name}.`);
      } else if (status === 'error') {
        message.error(`Nie udało się przesłać pliku ${info.file.name}.`);
      }
    },
    onDrop(e) {
      console.log('Dropped files', e.dataTransfer.files);
    },
  };

const RecognisePage = () => {
    const [form] = Form.useForm();
    const [usedModel, setUsedModel] = useState("Cyfry arabskie");

    const onFinish = (values: any) => {
        console.log('Received values of form: ', values);
    };

    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{paddingBottom: "100px"}}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Rozpoznawanie znaku</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Card bordered={true} style={{width: "40vw", boxShadow: '0 3px 10px rgb(0 0 0 / 0.2)' }}>
                                <Row justify="space-around" align="middle">
                                    <Form 
                                        layout='horizontal'
                                        form={form}
                                        name="train_options"
                                        className="train-form"
                                        onFinish={onFinish}
                                    >                                    
                                        
                                        <Row justify="end" align="middle" style={{width: "auto"}}>
                                            <Form.Item label="Używany model: ">
                                                <Space>
                                                    <Form.Item
                                                        name="distributionType"
                                                        noStyle
                                                        rules={[{ required: true, message: 'Pole jest wymagane' }]}
                                                    >
                                                        <Select style={{width: "11vw" }} onChange={setUsedModel} placeholder="Wybierz model..." options={Models} />
                                                    </Form.Item>
                                                    <Tooltip title="Tu wyświetla się instrukcja dla użytkownika.">
                                                        <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                                    </Tooltip>
                                                </Space>
                                            </Form.Item>
                                        </Row>
                                        <Row style={{width: "auto", marginBottom: "45px"}}>
                                            <Dragger {...props} maxCount={1} accept='image/png, image/jpeg, image/jpg, image/bmp, image/exif, image/tiff' style={{width: "30vw"}}>
                                                <p className="ant-upload-drag-icon">
                                                <InboxOutlined />
                                                </p>
                                                <p className="ant-upload-text">Kliknij lub przeciągnij plik aby załadować</p>
                                                <p className="ant-upload-hint">
                                                    Załącz jeden plik graficzny ze wzorcem do rozpoznania. <br />
                                                    Obsługiwane formaty: .jpg, .jpeg, .png, .bmp, .exif, .tiff.
                                                </p>
                                            </Dragger>
                                        </Row>
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
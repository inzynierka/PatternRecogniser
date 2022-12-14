import 'antd/dist/antd.min.css';

import { InboxOutlined, QuestionCircleOutlined } from '@ant-design/icons';
import { Button, Card, Col, Form, message, Row, Select, Space, Tooltip, Typography, Upload, UploadProps } from 'antd';
import { RcFile, UploadFile } from 'antd/lib/upload/interface';
import { useEffect, useState } from 'react';

import { ApiService, FileParameter } from '../../generated/ApiService';
import useWindowDimensions from '../../UseWindowDimensions';
import { Loading } from '../Common/Loading';



const { Dragger } = Upload;
const { Title } = Typography;

interface selectOption {
    value : string,
    label : string
}

const RecognisePage = () => {
    const apiService = new ApiService();
    const [form] = Form.useForm();
    const isOrientationVertical  = useWindowDimensions();
    const [usedModel , setUsedModel] = useState("");
    const [selectOptions, setSelectOptions] = useState<selectOption[]>([])
    const [patternFile, setPatternFile] = useState<UploadFile<any> | undefined>()
    const [loading, setLoading] = useState(false);

    const props: UploadProps = {
        onRemove: () => {
            setPatternFile(undefined);
        },
        // onDrop(e) {
        //   console.log('Dropped files', e.dataTransfer.files);
        // },
        beforeUpload: file => {
            //console.log("Before upload", file)
            setPatternFile(file);
            return false;
        },
      };

    const parseModelData = (data : any) => {
        let options : selectOption[] = [];
        let option : selectOption;

        data.forEach((item: any) => {
            option = {
                label: item.name,
                value: item.name
            }
            options.push(option);
        })

        return options;
    }
    const fetchModels = () => {
        setLoading(true);
        apiService.getModels()
            .then(response => response.json())
            .then(
                (data) => {
                    if(data !== undefined) {
                        const options = parseModelData(data);
                        setSelectOptions(options);
                    }
                    setLoading(false);
                }
            )
        return;
    }

    useEffect(() => {
        fetchModels();
    }, [])

    const onFinish = (values: any) => {
        apiService.patternRecognition(usedModel, patternFile as RcFile)
        .then(response => response.json())
        .then(
            (data) => {
                message.success("Operacja zakończona pomyślnie, wyświetlenie wyniku jest WIP");
                console.log(data);
            },
            (error) => {
                message.error("Wystąpił błąd podczas przetwarzania danych");
                console.log(error);
            }
        )
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
                            <Card bordered={true} style={{width: isOrientationVertical ? "40vw" : "65vw", boxShadow: '0 3px 10px rgb(0 0 0 / 0.2)', paddingTop: '20px' }}>
                                {
                                    loading ? 
                                    <Loading />
                                    :
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
                                                            name="usedModel"
                                                            noStyle
                                                            rules={[{ required: true, message: 'Pole jest wymagane' }]}
                                                        >
                                                            <Select style={{width: "11vw" }} onChange={setUsedModel} placeholder="Wybierz model..." options={selectOptions} />
                                                        </Form.Item>
                                                        <Tooltip title="Wybierz nazwę modelu, którego chcesz użyć do rozpoznania wzorca.">
                                                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                                        </Tooltip>
                                                    </Space>
                                                </Form.Item>
                                            </Row>
                                            <Row style={{width: "auto", marginBottom: "25px"}}>
                                                <Dragger {...props} maxCount={1} accept='image/png, image/jpeg, image/jpg, image/bmp, image/exif, image/tiff' style={{width: isOrientationVertical ? "30vw" : "50vw"}}>
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

                                            <Row justify='end' align="middle">
                                                <Form.Item>
                                                    <Button
                                                        type="primary"
                                                        data-testid="train-button"
                                                        htmlType="submit"
                                                        disabled={usedModel.length === 0 || patternFile === undefined}
                                                        style={{ marginTop: "15px", marginBottom: "14px" }}
                                                    >
                                                        Rozpocznij rozpoznawanie
                                                    </Button>
                                                </Form.Item>
                                            </Row>
                                        </Form>
                                    </Row>
                                }
                            </Card>
                        </Row>
                    </div>      
                </Col>
            </Row>
        </div>
    );
}

export default RecognisePage;
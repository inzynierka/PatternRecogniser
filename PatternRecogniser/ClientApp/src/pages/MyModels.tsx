import {Typography, Form, Card, Space, Select, Tooltip, UploadProps, Input, Button } from "antd"

import 'antd/dist/antd.min.css';
import { Row, Col, message, Upload } from "antd";
import { useState } from "react";
import { QuestionCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { Model } from "../classes/Model";
import ModelListElement from "./ModelListElement";

const { Title } = Typography;

const exampleModels : Model[] = [
    {
        name: "Cyfry arabskie",
        patternNum: 10
    },
    {
        name: "Alfabet",
        patternNum: 32
    },
    {
        name: "Figury geometryczne",
        patternNum: 56
    }
]

const MyModelsPage = () => {
    const [form] = Form.useForm();
    const [models, setModels] = useState(exampleModels)
    
    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        console.log(searchName)
        setModels(exampleModels.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }

    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{minHeight: "76vh" }}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Moje modele</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Row justify="space-between" align="middle" style={{width: "80vw", marginBottom: '20px'}}>
                                    <Space>
                                        <Input placeholder="Wyszukaj" prefix={<SearchOutlined />} onPressEnter={filter}/>
                                        <Tooltip title="Wciśnij ENTER aby wyszukać modelu po nazwie.">
                                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                        </Tooltip>
                                    </Space>
                                <Col>
                                    <Button type="default">Dodaj nowy model</Button>
                                </Col>
                            </Row>

                            <Row justify="space-around" align="middle">
                                <Card bordered={true} style={{width: "80vw"}}>
                                    {
                                        models.map((item: Model) => (<ModelListElement model={item}/> ))
                                    }
                                </Card>
                            </Row>
                        </Row>
                    </div>      
                </Col>
            </Row>
        </div>
    );
}

export default MyModelsPage;
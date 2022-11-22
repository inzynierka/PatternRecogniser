import {Typography, Card, Space, Tooltip, Input, Button } from "antd"

import 'antd/dist/antd.min.css';
import { Row, Col } from "antd";
import { useState } from "react";
import { QuestionCircleOutlined, SearchOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import { ModelType } from "../types/ModelType";
import ModelListElement from "./ModelListElement";
import { useNavigate } from 'react-router-dom';
import { Urls } from "../types/Urls";

const { Title } = Typography;

const exampleModels : ModelType[] = [
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

const CreateComparisonListPage = () => {
    const navigate = useNavigate();
    const [models, setModels] = useState(exampleModels)
    
    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        console.log(searchName)
        setModels(exampleModels.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }

    const goBackHandler = () => {
        navigate(Urls.ComparisonLists, {replace: true});
    }

    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{minHeight: "75vh" }}>
                        <Button onClick={goBackHandler} icon={<ArrowLeftOutlined style={{fontSize: '2em'}}/>} size="large" shape="circle" type="text" style={{marginLeft: '20px', marginTop: '15px'}}/>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px", marginTop: "-55px"}}>
                            <Title>Stwórz nową listę</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Row justify="space-between" align="middle" style={{width: "80vw", marginBottom: '20px'}}>
                                    <p></p>
                                <Col>
                                    <Space>
                                        <Input placeholder="Wyszukaj" prefix={<SearchOutlined />} onPressEnter={filter}/>
                                        <Tooltip title="Wciśnij ENTER aby wyszukać modelu po nazwie.">
                                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                        </Tooltip>
                                    </Space>
                                </Col>
                            </Row>

                            <Row justify="space-around" align="middle">
                                <Card bordered={true} style={{width: "80vw"}}>
                                    {
                                        models.map((item: ModelType) => (<ModelListElement model={item} addingToList={true}/> ))
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

export default CreateComparisonListPage;
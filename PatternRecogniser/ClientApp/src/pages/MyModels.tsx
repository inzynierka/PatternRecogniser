import 'antd/dist/antd.min.css';

import { QuestionCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { Button, Card, Col, Input, Row, Space, Tooltip, Typography } from 'antd';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { ModelType } from '../types/ModelType';
import { Urls } from '../types/Urls';
import ModelListElement from './ModelListElement';

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

const MyModelsPage = () => {
    const navigate = useNavigate();
    const [models, setModels] = useState(exampleModels)
    
    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        console.log(searchName)
        setModels(exampleModels.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }

    const addNewModelHandler = () => {
        navigate(Urls.Train, {replace: true});
    }

    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{minHeight: "75vh" }}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Moje modele</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Row justify="space-between" align="middle" style={{width: "80vw", marginBottom: '20px'}}>
                                    <Space>
                                        <Input data-testid="search-input" placeholder="Wyszukaj" prefix={<SearchOutlined />} onPressEnter={filter}/>
                                        <Tooltip title="Wciśnij ENTER aby wyszukać modelu po nazwie.">
                                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                        </Tooltip>
                                    </Space>
                                <Col>
                                    <Button data-testid="add-model-button" type="default" onClick={addNewModelHandler}>Dodaj nowy model</Button>
                                </Col>
                            </Row>

                            <Row justify="space-around" align="middle">
                                <Card data-testid="model-list-card" bordered={true} style={{width: "80vw"}}>
                                    {
                                        models.map((item: ModelType) => (<ModelListElement model={item}/> ))
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
import 'antd/dist/antd.min.css';

import { QuestionCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { Button, Card, Col, Input, Row, Space, Tooltip, Typography } from 'antd';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { ComparisonListType } from '../../types/ComparisonType';
import { Urls } from '../../types/Urls';
import ComparisonList from './ComparisonList';

const { Title } = Typography;

const exampleLists : ComparisonListType[] = [
    {
        name: "Modele z alfabetami",
        elementNum: 3
    },
    {
        name: "Rozpoznawanie cyfr",
        elementNum: 2,
        usedModel: "Cyfry arabskie"
    }
]

const ComparisonPage = () => {
    const navigate = useNavigate();
    const [lists, setLists] = useState(exampleLists)

    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        console.log(searchName)
        setLists(exampleLists.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }

    const createNewListHandler = () => {
        navigate(Urls.ComparisonListsCreate, {replace: true});
    }

    return (
        <div>
            <Row style={{ marginTop: 50 }}>
            <Col flex="auto">
                    <div className="site-layout-content" style={{minHeight: "75vh" }}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Porównywarka</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Row justify="space-between" align="middle" style={{width: "80vw", marginBottom: '20px'}}>
                                    <Space>
                                        <Input placeholder="Wyszukaj" prefix={<SearchOutlined />} onPressEnter={filter}/>
                                        <Tooltip title="Wciśnij ENTER aby wyszukać listy po nazwie.">
                                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                        </Tooltip>
                                    </Space>
                                <Col>
                                    <Button type="default" onClick={createNewListHandler}>Stwórz nową listę</Button>
                                </Col>
                            </Row>

                            <Row justify="space-around" align="middle">
                                <Card bordered={true} style={{width: "80vw"}}>
                                    {
                                        lists.map((item: ComparisonListType) => ( <ComparisonList list={item} /> ))
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

export default ComparisonPage;
import 'antd/dist/antd.min.css';

import { Button, Card, Col, Row, Typography } from 'antd';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { ComparisonListType } from '../../types/ComparisonType';
import { Urls } from '../../types/Urls';
import { SearchBar } from '../Common/SearchBar';
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
                            <SearchBar 
                                onPressEnterHandler={filter} 
                                button={<Button type="default" onClick={createNewListHandler}>Stwórz nową listę</Button>}
                            />

                            <Row justify="space-around" align="middle">
                                <Card bordered={true} style={{width: "80vw"}}>
                                    {
                                        lists.map((item: ComparisonListType) => ( <ComparisonList list={item} key={item.name}/> ))
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
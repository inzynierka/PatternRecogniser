import 'antd/dist/antd.min.css';

import { Button, Card, Col, Row, Typography } from 'antd';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { ComparisonListType, ExperimentType } from '../../types/ComparisonType';
import { Urls } from '../../types/Urls';
import { SearchBar } from '../Common/SearchBar';
import ComparisonList from './ComparisonList';
import { Loading } from '../Common/Loading';
import { NoData } from '../Common/NoData';
import { ApiService } from '../../generated/ApiService';

const { Title } = Typography;


const ComparisonPage = () => {
    const apiService = new ApiService();
    const navigate = useNavigate();
    const [lists, setLists] = useState<ComparisonListType[]>([]);
    const [displayedLists, setDisplayedLists] = useState<ComparisonListType[]>([]);
    const [loading, setLoading] = useState(false);
    const [dataLoaded, setDataLoaded] = useState(false);

    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        setDisplayedLists(lists.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }

    const createNewListHandler = () => {
        navigate(Urls.ComparisonListsCreate, {replace: true});
    }

    const parseListsData = (data : any) => {
        let lists : ComparisonListType[] = [];
        let list : ComparisonListType;

        data.forEach((item: any) => {
            list = {
                name: item.name,
                elementNum: item.experiments === null ? 0 : item.experiments.length(),
                experimentType: item.experimentType.includes("Pattern") ? ExperimentType.PatternRecognition : ExperimentType.ModelTraining,
                experimentListId: item.experimentListId
            }
            lists.push(list);
        })

        return lists;
    }

    const fetchLists = () => {
        setLoading(true);
        apiService.getLists()
            .then(response => response.json())
            .then(
                (data) => {
                    if(data !== undefined) {
                        const lists = parseListsData(data);
                        setLists(lists);
                        setDisplayedLists(lists);
                        setDataLoaded(true);
                    }
                    else setDataLoaded(false);
                    setLoading(false);
                }
            )
        return;
    }

    useEffect(() => {
        fetchLists();
    }, [])

    const deleteListHandler = (listName : string) => {
        console.log("unimplemented, Deleting list:", listName);
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
                                <Card data-testid="comparison-list-card" bordered={true} style={{width: "80vw"}}>
                                    {
                                        loading ? 
                                        <Loading />
                                        :
                                        displayedLists.length > 0 && dataLoaded ?
                                            displayedLists.map((item: ComparisonListType) => ( <ComparisonList list={item} key={item.name} deleteList={deleteListHandler}/> ))
                                            :
                                            <NoData />
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
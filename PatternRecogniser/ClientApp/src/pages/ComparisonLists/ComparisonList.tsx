import 'antd/dist/antd.min.css';

import { DeleteOutlined } from '@ant-design/icons';
import { Button, Card, Col, Row, Typography } from 'antd';

import { ComparisonListType, ExperimentType } from '../../types/ComparisonType';

const { Title } = Typography;

interface Props {
    list: ComparisonListType
}

const ComparisonList = (props: Props) => {
    const displayElementNumber = ()  => {
        let num = props.list.elementNum
        let list = props.list

        if(list.experimentType === ExperimentType.PatternRecognition) {
            if(num === 1) return num.toString() + " znak"
            if(num % 10 >= 2 && num % 10 <= 4) return num.toString() + " znaki"
            return num.toString() + " znaków"
        }
        else {
            if(num === 1) return num.toString() + " symbol"
            if(num % 10 >= 2 && num % 10 <= 4) return num.toString() + " symbole"
            return num.toString() + " symboli"
        }
        
    }

    return (
        <div key={"div_" + props.list.name}>
            <Card>
                <Row justify="space-between" align="middle">
                    <Col>
                        <Title data-testid="conjugatable-text" level={3}>{props.list.name}</Title>
                        {displayElementNumber()}
                        {
                            props.list.usedModel?.valueOf !== undefined &&
                            <p data-testid="model-name" style={{marginTop: "20px", fontSize: '1.1em', marginBottom: '0px'}}>Model: {props.list.usedModel}</p>
                        }
                    </Col>
                    <Col>
                        <Row justify="end">
                            <Button data-testid="details-button" type="primary" style={{width: "100px", marginBottom: '10px'}} size="large" key={"detailsButton_" + props.list.name}>Szczegóły</Button>
                        </Row>
                        <Row justify="end">
                            <Button data-testid="delete-button" type="default" shape="circle" icon={<DeleteOutlined />} key={"deleteButton" + props.list.name} size="large" />
                        </Row>
                    </Col>
                </Row>
            </Card>
        </div>
    );
}

export default ComparisonList;
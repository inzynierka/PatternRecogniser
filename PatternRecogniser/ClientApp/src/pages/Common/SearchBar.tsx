import 'antd/dist/antd.min.css';

import { QuestionCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { Col, Input, Row, Space, Tooltip, Typography } from 'antd';
import { ReactElement } from 'react';

interface Props {
    onPressEnterHandler : (e : any) => void,
    button : ReactElement<any, any>
}

export const SearchBar = (props : Props) => {
    return (
        <Row justify="space-between" align="middle" style={{width: "80vw", marginBottom: '20px'}}>
                <Space>
                    <Input data-testid="search-input" placeholder="Wyszukaj" prefix={<SearchOutlined />} onPressEnter={props.onPressEnterHandler}/>
                    <Tooltip title="Wciśnij ENTER aby wyszukać modelu po nazwie.">
                        <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                    </Tooltip>
                </Space>
            <Col>
                props.button
            </Col>
        </Row>
    )
}
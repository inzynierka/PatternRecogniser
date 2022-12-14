import 'antd/dist/antd.min.css';

import { QuestionCircleOutlined } from '@ant-design/icons';
import { Form, Input, Modal, Space, Tooltip, Typography } from 'antd';
import { useState } from 'react';

import useWindowDimensions from '../../UseWindowDimensions';

interface Props {
    modalOpen : boolean,
    onOkHandler : Function,
    onCancelHandler : Function,
}

export const CreateComparisonListModal = (props : Props) => {
    const [listName, setListName] = useState("");
    const isOrientationVertical  = useWindowDimensions();

    const onOkHandler = () => {
        props.onOkHandler(listName);
    };

    return (
        <Modal
            title="Wprowadź nazwę nowej listy"
            centered
            visible={props.modalOpen}
            onOk={onOkHandler}
            onCancel={(e) => props.onCancelHandler()}
        >
            <Form 
                    layout='horizontal'
                    name="train_options"
                    className="train-form"
                    data-testid="train-form"
                >     
                <Form.Item label="Nazwa modelu: " style={{width: isOrientationVertical ? "15vw" : "40vw" }}>
                    <Space>
                        <Form.Item
                            name="modelName"
                            noStyle
                            validateTrigger={['onBlur']}
                            rules={[{ required: true, message: 'Nazwa listy nie może być pusta.' }]}
                        >
                            <Input placeholder='Wpisz nazwę modelu' data-testid="model-name-input" style={{width: "18.1vw" }} onChange={(e) => setListName(e.target.value)}/>
                        </Form.Item>
                        <Tooltip title="Nazwa listy musi być unikalna." data-testid="name-tooltip">
                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                        </Tooltip>
                    </Space>
                </Form.Item>  
            </Form>
        </Modal>
    )
}
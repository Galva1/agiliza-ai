import { useEffect, useRef } from "react";
import { EditorContent, useEditor } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import Image from "@tiptap/extension-image";
import Placeholder from "@tiptap/extension-placeholder";
import Underline from "@tiptap/extension-underline";

interface RichTextEditorProps {
  value: string;
  onChange: (html: string) => void;
  placeholder?: string;
  readOnly?: boolean;
}

function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result as string);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

export function RichTextEditor({ value, onChange, placeholder, readOnly }: RichTextEditorProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);

  const editor = useEditor({
    extensions: [
      StarterKit,
      Underline,
      Image.configure({ inline: false, HTMLAttributes: { class: "rounded-lg" } }),
      Placeholder.configure({ placeholder: placeholder ?? "Escreva aqui..." }),
    ],
    content: value,
    editable: !readOnly,
    editorProps: {
      handlePaste: (view, event) => {
        const files = Array.from(event.clipboardData?.files ?? []).filter((file) =>
          file.type.startsWith("image/")
        );
        if (files.length === 0) return false;
        event.preventDefault();
        files.forEach((file) => {
          fileToBase64(file).then((base64) => {
            const { schema } = view.state;
            const node = schema.nodes.image.create({ src: base64 });
            const transaction = view.state.tr.replaceSelectionWith(node);
            view.dispatch(transaction);
          });
        });
        return true;
      },
      handleDrop: (view, event) => {
        const files = Array.from(event.dataTransfer?.files ?? []).filter((file) =>
          file.type.startsWith("image/")
        );
        if (files.length === 0) return false;
        event.preventDefault();
        files.forEach((file) => {
          fileToBase64(file).then((base64) => {
            const { schema } = view.state;
            const node = schema.nodes.image.create({ src: base64 });
            const transaction = view.state.tr.replaceSelectionWith(node);
            view.dispatch(transaction);
          });
        });
        return true;
      },
    },
    onUpdate: ({ editor: current }) => {
      onChange(current.getHTML());
    },
  });

  useEffect(() => {
    if (!editor) return;
    if (value !== editor.getHTML()) {
      editor.commands.setContent(value, { emitUpdate: false });
    }
  }, [value, editor]);

  useEffect(() => {
    editor?.setEditable(!readOnly);
  }, [readOnly, editor]);

  if (!editor) return null;

  const handleImageButtonClick = () => {
    fileInputRef.current?.click();
  };

  const handleFileSelected = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;
    const base64 = await fileToBase64(file);
    editor.chain().focus().setImage({ src: base64 }).run();
    event.target.value = "";
  };

  const toolbarButtonClass = (active: boolean) =>
    `flex h-8 min-w-[32px] items-center justify-center rounded-md px-2 text-sm font-semibold transition-colors ${
      active
        ? "bg-brand-600 text-white"
        : "text-surface-600 hover:bg-surface-200 dark:text-surface-300 dark:hover:bg-surface-700"
    }`;

  return (
    <div>
      {!readOnly && (
        <div className="flex flex-wrap items-center gap-1 rounded-t-lg border border-surface-300 bg-surface-50 px-2 py-1.5 dark:border-surface-700 dark:bg-surface-900">
          <button
            type="button"
            onClick={() => editor.chain().focus().toggleBold().run()}
            className={toolbarButtonClass(editor.isActive("bold"))}
            title="Negrito"
          >
            B
          </button>
          <button
            type="button"
            onClick={() => editor.chain().focus().toggleItalic().run()}
            className={`${toolbarButtonClass(editor.isActive("italic"))} italic`}
            title="Itálico"
          >
            I
          </button>
          <button
            type="button"
            onClick={() => editor.chain().focus().toggleUnderline().run()}
            className={`${toolbarButtonClass(editor.isActive("underline"))} underline`}
            title="Sublinhado"
          >
            S
          </button>
          <span className="mx-1 h-5 w-px bg-surface-300 dark:bg-surface-700" />
          <button
            type="button"
            onClick={() => editor.chain().focus().toggleHeading({ level: 3 }).run()}
            className={toolbarButtonClass(editor.isActive("heading", { level: 3 }))}
            title="Título"
          >
            H
          </button>
          <button
            type="button"
            onClick={() => editor.chain().focus().setParagraph().run()}
            className={toolbarButtonClass(editor.isActive("paragraph"))}
            title="Parágrafo"
          >
            P
          </button>
          <span className="mx-1 h-5 w-px bg-surface-300 dark:bg-surface-700" />
          <button
            type="button"
            onClick={() => editor.chain().focus().toggleBulletList().run()}
            className={toolbarButtonClass(editor.isActive("bulletList"))}
            title="Lista com marcadores"
          >
            •
          </button>
          <button
            type="button"
            onClick={() => editor.chain().focus().toggleOrderedList().run()}
            className={toolbarButtonClass(editor.isActive("orderedList"))}
            title="Lista numerada"
          >
            1.
          </button>
          <span className="mx-1 h-5 w-px bg-surface-300 dark:bg-surface-700" />
          <button
            type="button"
            onClick={handleImageButtonClick}
            className={toolbarButtonClass(false)}
            title="Inserir imagem"
          >
            🖼
          </button>
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            className="hidden"
            onChange={handleFileSelected}
          />
        </div>
      )}
      <EditorContent editor={editor} />
    </div>
  );
}

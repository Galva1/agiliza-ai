export function RichTextView({ html }: { html: string }) {
  return <div className="rich-text-content" dangerouslySetInnerHTML={{ __html: html }} />;
}
